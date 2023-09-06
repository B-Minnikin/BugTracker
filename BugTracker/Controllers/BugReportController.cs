using BugTracker.Helpers;
using BugTracker.Models;
using BugTracker.Services;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BugTracker.Database.Repository.Interfaces;
using BugTracker.Models.Authorization;
using BugTracker.Models.Subscription;

namespace BugTracker.Controllers
{
	[Authorize]
	public class BugReportController : Controller
	{
		private readonly IProjectRepository projectRepository;
		private readonly IMilestoneRepository milestoneRepository;
		private readonly IBugReportRepository bugReportRepository;
		private readonly IBugReportStatesRepository bugReportStatesRepository;
		private readonly IUserSubscriptionsRepository userSubscriptionsRepository;
		private readonly IActivityRepository activityRepository;
		private readonly ICommentRepository commentRepository;
		private readonly IAuthorizationService authorizationService;
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly ISubscriptions subscriptions;
		private readonly LinkGenerator linkGenerator;
		private readonly ApplicationLinkGenerator applicationLinkGenerator;
		private readonly ApplicationUserManager userManager;

		public BugReportController(IProjectRepository projectRepository,
										  IMilestoneRepository milestoneRepository,
										  IBugReportRepository bugReportRepository,
										  IBugReportStatesRepository bugReportStatesRepository,
										  IUserSubscriptionsRepository userSubscriptionsRepository,
										  IActivityRepository activityRepository,
										  ICommentRepository commentRepository,
										  IAuthorizationService authorizationService,
										  IHttpContextAccessor httpContextAccessor,
										  ISubscriptions subscriptions,
										  LinkGenerator linkGenerator,
										  ApplicationUserManager userManager)
		{
			this.projectRepository = projectRepository;
			this.milestoneRepository = milestoneRepository;
			this.bugReportRepository = bugReportRepository;
			this.bugReportStatesRepository = bugReportStatesRepository;
			this.userSubscriptionsRepository = userSubscriptionsRepository;
			this.activityRepository = activityRepository;
			this.commentRepository = commentRepository;
			this.authorizationService = authorizationService;
			this.httpContextAccessor = httpContextAccessor;
			this.subscriptions = subscriptions;
			this.linkGenerator = linkGenerator;
			this.userManager = userManager;

			this.applicationLinkGenerator = new ApplicationLinkGenerator(httpContextAccessor, linkGenerator);
		}

		[HttpGet]
		public async Task<IActionResult> CreateReport()
		{
			var currentProjectId = httpContextAccessor.HttpContext?.Session.GetInt32("currentProject");
			if (!currentProjectId.HasValue) return BadRequest();
			
			var currentProject = await projectRepository.GetById(currentProjectId.Value);

			var authorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext?.User, currentProjectId, "CanAccessProjectPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.BugReportCreate(currentProject);

				return View();
			}

			return RedirectToAction("Overview", "Projects", new { projectId = currentProjectId });
		}

		[HttpPost]
		public async Task<IActionResult> CreateReport(CreateBugReportViewModel viewModel)
		{
			var currentProjectId = httpContextAccessor.HttpContext?.Session.GetInt32("currentProject") ?? 0;
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProjectId, "CanAccessProjectPolicy");
			if (!authorizationResult.IsCompletedSuccessfully || !authorizationResult.Result.Succeeded)
			{
				return Forbid();
			}

			if (!ModelState.IsValid)
			{
				return View();
			}

			var name = httpContextAccessor.HttpContext?.User.Identity?.Name ?? "N/A";
			
			var newBugReport = new BugReport
			{
				Title = viewModel.Title,
				ProgramBehaviour = viewModel.ProgramBehaviour,
				DetailsToReproduce = viewModel.DetailsToReproduce,
				CreationTime = DateTime.Now,
				Hidden = viewModel.Hidden,
				Severity = viewModel.Severity,
				Importance = viewModel.Importance,
				PersonReporting = name,
				ProjectId = currentProjectId
			};

			// add bug report to current project
			var bugReport = await bugReportRepository.Add(newBugReport);

			// Create activity event
			var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			
			var commentActivity = new ActivityBugReport(DateTime.Now, currentProjectId, ActivityMessage.BugReportPosted, userId, bugReport.BugReportId);
			await activityRepository.Add(commentActivity);

			var bugState = new BugState
			{
				Time = DateTime.Now,
				StateType = StateType.Open,
				Author = httpContextAccessor.HttpContext?.User.Identity?.Name,
				BugReportId = bugReport.BugReportId
			};
			_ = await bugReportStatesRepository .Add(bugState);
			
			// deal with subscriptions after bug states to prevent premature email updates
			if (viewModel.Subscribe && !await subscriptions.IsSubscribed(userId, bugReport.BugReportId))
			{
				// add to subscriptions in the repo
				await userSubscriptionsRepository.AddSubscription(userId, bugReport.BugReportId);
			}

			return RedirectToAction("ReportOverview", new { id = bugReport.BugReportId });
		}

		public IActionResult Subscribe(int bugReportId)
		{
			var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			
			subscriptions.CreateSubscriptionIfNotSubscribed(userId, bugReportId);

			return RedirectToAction("ReportOverview", new { id = bugReportId});
		}

		[HttpGet]
		public async Task<IActionResult> Edit(int bugReportId)
		{
			var latestState = await bugReportStatesRepository.GetLatestState(bugReportId);

			var viewModel = new EditBugReportViewModel
			{
				BugReport = await bugReportRepository.GetById(bugReportId),
				CurrentState = latestState.StateType
			};

			var currentProjectId = httpContextAccessor.HttpContext?.Session.GetInt32("currentProject") ?? 0;
			if (currentProjectId == 0) return BadRequest();
			var currentProject = await projectRepository.GetById(currentProjectId);

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, new { ProjectId = currentProjectId, viewModel.BugReport.PersonReporting}, "CanModifyReportPolicy");
			if (!authorizationResult.IsCompletedSuccessfully && !authorizationResult.Result.Succeeded)
			{
				return RedirectToAction("Overview", "Projects", new { id = currentProjectId});
			}
			
			ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.BugReportEdit(currentProject, viewModel.BugReport);

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(EditBugReportViewModel viewModel)
		{
			if (viewModel == null) return BadRequest();
			
			if (!ModelState.IsValid)
			{
				return View(viewModel);
			}
			
			var bugReport = await bugReportRepository.GetById(viewModel.BugReport.BugReportId);
			if (bugReport == null) return NotFound();
			
			bugReport.Title = viewModel.BugReport.Title;
			bugReport.DetailsToReproduce = viewModel.BugReport.DetailsToReproduce;
			bugReport.ProgramBehaviour = viewModel.BugReport.ProgramBehaviour;
			bugReport.Severity = viewModel.BugReport.Severity;
			bugReport.Importance = viewModel.BugReport.Importance;
			bugReport.Hidden = viewModel.BugReport.Hidden;
			bugReport.CreationTime = viewModel.BugReport.CreationTime;

			_ = bugReportRepository.Update(bugReport);

			// Create activity event
			var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			
			var currentProjectId = HttpContext.Session.GetInt32("currentProject");
			if (!currentProjectId.HasValue) return BadRequest();
			
			var activityEvent = new ActivityBugReport(DateTime.Now, currentProjectId.Value, ActivityMessage.BugReportEdited, userId, bugReport.BugReportId);
			await activityRepository.Add(activityEvent);

			var latestBugState = await bugReportStatesRepository.GetLatestState(bugReport.BugReportId);
			if (!viewModel.CurrentState.Equals(latestBugState.StateType))
			{
				var newBugState = new BugState
				{
					Time = DateTime.Now,
					StateType = viewModel.CurrentState,
					Author = HttpContext.User.Identity?.Name ?? "N/A",
					BugReportId = bugReport.BugReportId
				};

				var createdBugState = await bugReportStatesRepository.Add(newBugState);

				var linkGen = new ApplicationLinkGenerator(httpContextAccessor, linkGenerator);
				await subscriptions.NotifyBugReportStateChanged(createdBugState, linkGen, bugReport.BugReportId);

				// Create activity event
				var stateActivityEvent = new ActivityBugReportStateChange(DateTime.Now, currentProjectId.Value, ActivityMessage.BugReportStateChanged, userId, bugReport.BugReportId, createdBugState.BugStateId, latestBugState.BugStateId);
				await activityRepository.Add(stateActivityEvent);
			}
			
			return RedirectToAction("ReportOverview", new { id = bugReport.BugReportId});
		}

		public async Task<IActionResult> Delete(int bugReportId)
		{
			var currentProjectId = httpContextAccessor.HttpContext?.Session.GetInt32("currentProject");
			if (!currentProjectId.HasValue) return BadRequest();
			
			var bugReport = await bugReportRepository.GetById(bugReportId);

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, new { ProjectId = currentProjectId, bugReport.PersonReporting }, "CanModifyReportPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				await bugReportRepository.Delete(bugReportId);
			}

			return RedirectToAction("Overview", "Projects", new { id = currentProjectId });
		}

		[HttpGet]
		public async Task<IActionResult> AssignMember(int bugReportId)
		{
			var currentProjectId = httpContextAccessor.HttpContext?.Session.GetInt32("currentProject");
			if (!currentProjectId.HasValue) return BadRequest();
			var bugReport = await bugReportRepository .GetById(bugReportId);

			var authorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User, currentProjectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				var currentProject = await projectRepository.GetById(currentProjectId.Value);
				ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.BugReportAssignMember(currentProject, bugReport);

				var assignedUsers = await bugReportRepository.GetAssignedUsersForBugReport(bugReportId);

				var viewModel = new AssignMemberViewModel()
				{
					BugReportId = bugReportId,
					ProjectId = currentProjectId.Value,
					AssignedUsers = assignedUsers.ToList()
				};

				return View(viewModel);
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public async Task<IActionResult> AssignMember(AssignMemberViewModel viewModel)
		{
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, viewModel.ProjectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				var assignedUser = await userManager.FindByEmailAsync(viewModel.MemberEmail);

				await bugReportRepository.AddUserAssignedToBugReport(assignedUser.Id, viewModel.BugReportId);

				// Create activity event
				var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				var currentProjectId = HttpContext.Session.GetInt32("currentProject");
				var activityEvent = new ActivityBugReportAssigned(DateTime.Now, currentProjectId.Value, ActivityMessage.BugReportAssignedToUser, userId, viewModel.BugReportId, assignedUser.Id);
				await activityRepository.Add(activityEvent);

				return RedirectToAction("AssignMember", new { viewModel.BugReportId });
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public async Task<IActionResult> RemoveAssignedMember(int projectId, int bugReportId, string memberEmail)
		{
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, projectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				var user = await userManager.FindByEmailAsync(memberEmail);

				await bugReportRepository.DeleteUserAssignedToBugReport(user.Id, bugReportId);

				return RedirectToAction("AssignMember", new { bugReportId });
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public async Task<IActionResult> ManageLinks(int bugReportId)
		{
			var currentProjectId = httpContextAccessor.HttpContext?.Session.GetInt32("currentProject");
			if (!currentProjectId.HasValue) return BadRequest();
			
			var bugReport = await bugReportRepository.GetById(bugReportId);

			var authorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User, currentProjectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				var currentProject = await projectRepository.GetById(currentProjectId.Value);
				ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.BugReportManageLinks(currentProject, bugReport);

				var linkedReports = await bugReportRepository.GetLinkedReports(bugReportId);

				var model = new ManageLinksViewModel {
					ProjectId = currentProjectId.Value,
					BugReportId = bugReportId,
					LinkedReports = linkedReports.ToList()
				};

				return View(model);
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public async Task<IActionResult> LinkReports(LinkReportsViewModel viewModel)
		{
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, viewModel.ProjectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				var linkToReport = await bugReportRepository.GetBugReportByLocalId(viewModel.LinkToBugReportLocalId, viewModel.ProjectId);
				await bugReportRepository.AddBugReportLink(viewModel.BugReportId, linkToReport.BugReportId);

				// Create activity event
				var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				var currentProjectId = HttpContext.Session.GetInt32("currentProject");
				if (!currentProjectId.HasValue) return BadRequest();
				
				var activityEvent = new ActivityBugReportLink(DateTime.Now, currentProjectId.Value, ActivityMessage.BugReportsLinked, userId, viewModel.BugReportId, linkToReport.BugReportId);
				await activityRepository.Add(activityEvent);

				return RedirectToAction("ReportOverview", new { id = viewModel.BugReportId });
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public async Task<IActionResult> DeleteLink(int projectId, int bugReportId, int linkToBugReportId)
		{
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, projectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				await bugReportRepository.DeleteBugReportLink(bugReportId, linkToBugReportId);

				return RedirectToAction("ReportOverview", new { id = bugReportId });
			}

			return RedirectToAction("Index", "Home");
		}

		public async Task<IActionResult> ReportOverview(int bugReportId)
		{
			var currentProjectId = HttpContext.Session.GetInt32("currentProject");

			var authorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User, currentProjectId, "CanAccessProjectPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				HttpContext.Session.SetInt32("currentBugReport", bugReportId);
				var bugReport = await bugReportRepository.GetById(bugReportId);

				var bugStates = await bugReportStatesRepository.GetAllById(bugReport.BugReportId);
				var bugStatesList = bugStates.OrderByDescending(o => o.Time).ToList();

				var assignedMembers = await bugReportRepository.GetAssignedUsersForBugReport(bugReportId);
				var assignedMembersList = assignedMembers.Select(x => new string(x.UserName)).ToList();

				var assignedMembersDisplay = assignedMembersList.Count > 0 
					? string.Join(", ", assignedMembersList)
					: "Unassigned";

				var comments = await commentRepository.GetAllById(bugReport.BugReportId);
				var activities = await activityRepository.GetBugReportActivities(bugReport.BugReportId);

				var viewModel = new OverviewBugReportViewModel
				{
					BugReport = bugReport,
					Comments = comments.ToList(),
					BugStates = bugStatesList,
					Activities = activities.ToList(),
					CurrentState = bugStatesList[0].StateType,
					AssignedMembersDisplay = assignedMembersDisplay
				};

				// generate activity messages
				var applicationLinkGenerator = new ApplicationLinkGenerator(httpContextAccessor, linkGenerator);
				var activityMessageBuilder = new ActivityMessageBuilder(applicationLinkGenerator, userManager, projectRepository, 
					bugReportRepository, milestoneRepository, bugReportStatesRepository);
				await activityMessageBuilder.GenerateMessages(viewModel.Activities);

				var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (await subscriptions.IsSubscribed(userId, viewModel.BugReport.BugReportId))
				{
					viewModel.DisableSubscribeButton = true;
				}
				var adminAuthorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User, currentProjectId, "ProjectAdministratorPolicy");
				if (adminAuthorizationResult.IsCompletedSuccessfully && adminAuthorizationResult.Result.Succeeded)
				{
					viewModel.DisableAssignMembersButton = false;
				}

				var currentProject = await projectRepository.GetById(currentProjectId ?? 0);

				ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.BugReportOverview(currentProject, bugReport.Title);

				return View(viewModel);
			}

			return RedirectToAction("Index", "Home");
		}
	}
}
