using BugTracker.Helpers;
using BugTracker.Models;
using BugTracker.Models.Database;
using BugTracker.Repository;
using BugTracker.Repository.Interfaces;
using BugTracker.Services;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
	[Authorize]
	public class BugReportController : Controller
	{
		private readonly ILogger<BugReportController> logger;
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
		private readonly UserManager<IdentityUser> userManager;

		public BugReportController(ILogger<BugReportController> logger,
									        IProjectRepository projectRepository,
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
										  UserManager<IdentityUser> userManager)
		{
			this.logger = logger;
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
		}

		[HttpGet]
		public async Task<ViewResult> CreateReport()
		{
			var currentProjectId = HttpContext.Session.GetInt32("currentProject");
			var currentProject = await projectRepository .GetById(currentProjectId ?? 0);

			ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.BugReportCreate(currentProject);

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CreateReport(CreateBugReportViewModel model)
		{
			int currentProjectId = (int)HttpContext.Session.GetInt32("currentProject");
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProjectId, "CanAccessProjectPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				if (ModelState.IsValid)
				{
					BugReport newBugReport = new BugReport
					{
						Title = model.Title,
						ProgramBehaviour = model.ProgramBehaviour,
						DetailsToReproduce = model.DetailsToReproduce,
						CreationTime = DateTime.Now,
						Hidden = model.Hidden, // to implement
						Severity = model.Severity,
						Importance = model.Importance,
						PersonReporting = HttpContext.User.Identity.Name,
						ProjectId = currentProjectId
					};

					// add bug report to current project
					BugReport addedReport = await bugReportRepository.Add(newBugReport);

					// Create activity event
					int userId = Int32.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
					var commentActivity = new ActivityBugReport(DateTime.Now, currentProjectId, ActivityMessage.BugReportPosted, userId, addedReport.BugReportId);
					await activityRepository.Add(commentActivity);

					BugState newBugState = new BugState
					{
						Time = DateTime.Now,
						StateType = StateType.open,
						Author = HttpContext.User.Identity.Name,
						BugReportId = addedReport.BugReportId
					};
					BugState addedBugState = await bugReportStatesRepository .Add(newBugState);
					
					// deal with subscriptions after bug states to prevent premature email updates
					if (model.Subscribe && !await subscriptions.IsSubscribed(userId, addedReport.BugReportId))
					{
						// add to subscriptions in the repo
						await userSubscriptionsRepository.AddSubscription(userId, addedReport.BugReportId);
					}

					return RedirectToAction("ReportOverview", new { id = addedReport.BugReportId });
				}
			}

			return View();
		}

		public IActionResult Subscribe(int bugReportId)
		{
			int userId = Int32.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

			subscriptions.CreateSubscriptionIfNotSubscribed(userId, bugReportId);

			return RedirectToAction("ReportOverview", new { id = bugReportId});
		}

		[HttpGet]
		public async Task<IActionResult> Edit(int bugReportId)
		{
			var latestState = await bugReportStatesRepository.GetLatestState(bugReportId);

			EditBugReportViewModel reportViewModel = new EditBugReportViewModel
			{
				BugReport = await bugReportRepository .GetById(bugReportId),
				CurrentState = latestState.StateType
			};

			var currentProjectId = HttpContext.Session.GetInt32("currentProject");
			var currentProject = await projectRepository .GetById(currentProjectId ?? 0);

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, new { ProjectId = currentProjectId, PersonReporting = reportViewModel.BugReport.PersonReporting}, "CanModifyReportPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.BugReportEdit(currentProject, reportViewModel.BugReport);

				return View(reportViewModel);
			}

			return RedirectToAction("Overview", "Projects", new { id = currentProjectId});
		}

		[HttpPost]
		public async Task<IActionResult> Edit(EditBugReportViewModel model)
		{
			if (ModelState.IsValid)
			{
				BugReport bugReport = await bugReportRepository .GetById(model.BugReport.BugReportId);
				bugReport.Title = model.BugReport.Title;
				bugReport.DetailsToReproduce = model.BugReport.DetailsToReproduce;
				bugReport.ProgramBehaviour = model.BugReport.ProgramBehaviour;
				bugReport.Severity = model.BugReport.Severity;
				bugReport.Importance = model.BugReport.Importance;
				bugReport.Hidden = model.BugReport.Hidden;
				bugReport.CreationTime = model.BugReport.CreationTime;

				_ = bugReportRepository.Update(bugReport);

				// Create activity event
				int userId = Int32.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
				var currentProjectId = HttpContext.Session.GetInt32("currentProject");
				var activityEvent = new ActivityBugReport(DateTime.Now, currentProjectId.Value, ActivityMessage.BugReportEdited, userId, bugReport.BugReportId);
				await activityRepository.Add(activityEvent);

				BugState latestBugState = await bugReportStatesRepository.GetLatestState(bugReport.BugReportId);
				if (!model.CurrentState.Equals(latestBugState.StateType))
				{
					BugState newBugState = new BugState
					{
						Time = DateTime.Now,
						StateType = model.CurrentState,
						Author = HttpContext.User.Identity.Name,
						BugReportId = bugReport.BugReportId
					};

					var createdBugState = await bugReportStatesRepository.Add(newBugState);

					string bugReportUrl = Url.Action("ReportOverview", "BugReport", new { id = bugReport.BugReportId }, Request.Scheme);
					await subscriptions .NotifyBugReportStateChanged(createdBugState, bugReportUrl);

					// Create activity event
					var stateActivityEvent = new ActivityBugReportStateChange(DateTime.Now, currentProjectId.Value, ActivityMessage.BugReportStateChanged, userId, bugReport.BugReportId, createdBugState.BugStateId, latestBugState.BugStateId);
					await activityRepository .Add(stateActivityEvent);
				}

				return RedirectToAction("ReportOverview", new { id = bugReport.BugReportId});
			}

			return View();
		}

		public async Task<IActionResult> Delete(int bugReportId)
		{
			int currentProjectId = (int)HttpContext.Session.GetInt32("currentProject");
			var bugReport = await bugReportRepository.GetById(bugReportId);

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, new { ProjectId = currentProjectId, PersonReporting = bugReport.PersonReporting }, "CanModifyReportPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				await bugReportRepository.Delete(bugReportId);
			}

			return RedirectToAction("Overview", "Projects", new { id = currentProjectId });
		}

		[HttpGet]
		public async Task<IActionResult> AssignMember(int bugReportId)
		{
			int currentProjectId = (int)HttpContext.Session.GetInt32("currentProject");
			var bugReport = await bugReportRepository .GetById(bugReportId);

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProjectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				var currentProject = await projectRepository.GetById(currentProjectId);
				ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.BugReportAssignMember(currentProject, bugReport);

				var assignedUsers = await bugReportRepository.GetAssignedUsersForBugReport(bugReportId);

				AssignMemberViewModel viewModel = new AssignMemberViewModel()
				{
					BugReportId = bugReportId,
					ProjectId = currentProjectId,
					AssignedUsers = assignedUsers.ToList()
				};

				return View(viewModel);
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public async Task<IActionResult> AssignMember(AssignMemberViewModel model)
		{
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, model.ProjectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				var user = await userManager.FindByEmailAsync(model.MemberEmail);
				var assignedUserId = Int32.Parse(user.Id);

				await bugReportRepository.AddUserAssignedToBugReport(assignedUserId, model.BugReportId);

				// Create activity event
				int userId = Int32.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
				var currentProjectId = HttpContext.Session.GetInt32("currentProject");
				var activityEvent = new ActivityBugReportAssigned(DateTime.Now, currentProjectId.Value, ActivityMessage.BugReportAssignedToUser, userId, model.BugReportId, assignedUserId);
				await activityRepository.Add(activityEvent);

				return RedirectToAction("AssignMember", new { model.BugReportId });
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

				await bugReportRepository.DeleteUserAssignedToBugReport(Int32.Parse(user.Id), bugReportId);

				return RedirectToAction("AssignMember", new { bugReportId });
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public async Task<IActionResult> ManageLinks(int bugReportId)
		{
			int currentProjectId = (int)HttpContext.Session.GetInt32("currentProject");
			var bugReport = await bugReportRepository.GetById(bugReportId);

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProjectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				var currentProject = await projectRepository.GetById(currentProjectId);
				ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.BugReportManageLinks(currentProject, bugReport);

				var linkedReports = await bugReportRepository.GetLinkedReports(bugReportId);

				ManageLinksViewModel model = new ManageLinksViewModel {
					ProjectId = currentProjectId,
					BugReportId = bugReportId,
					LinkedReports = linkedReports.ToList()
				};

				return View(model);
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public async Task<IActionResult> LinkReports(LinkReportsViewModel model)
		{
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, model.ProjectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				var linkToReport = await bugReportRepository.GetBugReportByLocalId(model.LinkToBugReportLocalId, model.ProjectId);
				await bugReportRepository.AddBugReportLink(model.BugReportId, linkToReport.BugReportId);

				// Create activity event
				int userId = Int32.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
				var currentProjectId = HttpContext.Session.GetInt32("currentProject");
				var activityEvent = new ActivityBugReportLink(DateTime.Now, currentProjectId.Value, ActivityMessage.BugReportsLinked, userId, model.BugReportId, linkToReport.BugReportId);
				await activityRepository .Add(activityEvent);

				return RedirectToAction("ReportOverview", new { id = model.BugReportId });
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public IActionResult DeleteLink(int projectId, int bugReportId, int linkToBugReportId)
		{
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, projectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				bugReportRepository.DeleteBugReportLink(bugReportId, linkToBugReportId);

				return RedirectToAction("ReportOverview", new { id = bugReportId });
			}

			return RedirectToAction("Index", "Home");
		}

		public async Task<IActionResult> ReportOverview(int id)
		{
			var currentProjectId = HttpContext.Session.GetInt32("currentProject");

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProjectId, "CanAccessProjectPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				HttpContext.Session.SetInt32("currentBugReport", id);
				BugReport bugReport = await bugReportRepository.GetById(id);

				var bugStates = await bugReportStatesRepository.GetAllById(bugReport.BugReportId);
				var bugStatesList = bugStates.OrderByDescending(o => o.Time).ToList();

				var assignedMembers = await bugReportRepository.GetAssignedUsersForBugReport(id);
				var assignedMembersList = assignedMembers.Select(x => new string(x.UserName)).ToList();

				string assignedMembersDisplay = "";
				if (assignedMembersList.Count > 0)
					assignedMembersDisplay = string.Join(", ", assignedMembersList);
				else
					assignedMembersDisplay = "Unassigned";

				var comments = await commentRepository.GetAllById(bugReport.BugReportId);
				var activities = await activityRepository.GetBugReportActivities(bugReport.BugReportId);

				OverviewBugReportViewModel bugViewModel = new OverviewBugReportViewModel
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
				activityMessageBuilder.GenerateMessages(bugViewModel.Activities);

				int userId = Int32.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
				if (await subscriptions.IsSubscribed(userId, bugViewModel.BugReport.BugReportId))
				{
					bugViewModel.DisableSubscribeButton = true;
				}
				var adminAuthorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProjectId, "ProjectAdministratorPolicy");
				if (adminAuthorizationResult.IsCompletedSuccessfully && adminAuthorizationResult.Result.Succeeded)
				{
					bugViewModel.DisableAssignMembersButton = false;
				}

				var currentProject = await projectRepository.GetById(currentProjectId ?? 0);

				ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.BugReportOverview(currentProject, bugReport.Title);

				return View(bugViewModel);
			}

			return RedirectToAction("Index", "Home");
		}
	}
}
