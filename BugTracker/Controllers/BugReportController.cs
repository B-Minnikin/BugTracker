using BugTracker.Models;
using BugTracker.Models.Authorization;
using BugTracker.Models.Database;
using BugTracker.Repository;
using BugTracker.Repository.Interfaces;
using BugTracker.Services;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using SmartBreadcrumbs.Attributes;
using SmartBreadcrumbs.Nodes;
using System;
using System.Collections.Generic;
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
		private readonly ApplicationUserManager userManager;

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
										  LinkGenerator linkGenerator)
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
			this.userManager = new ApplicationUserManager();
		}

		[HttpGet]
		public ViewResult CreateReport()
		{
			var currentProjectId = HttpContext.Session.GetInt32("currentProject");
			var currentProject = projectRepository.GetById(currentProjectId ?? 0);

			// --------------------- CONFIGURE BREADCRUMB NODES ----------------------------
			var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
			var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", currentProject.Name)
			{
				RouteValues = new { id = currentProjectId},
				Parent = projectsNode
			};
			var reportNode = new MvcBreadcrumbNode("CreateReport", "BugReport", "Create Bug Report")
			{
				Parent = overviewNode
			};
			ViewData["BreadcrumbNode"] = reportNode;
			// --------------------------------------------------------------------------------------------

			return View();
		}

		[HttpPost]
		public IActionResult CreateReport(CreateBugReportViewModel model)
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
					BugReport addedReport = bugReportRepository.Add(newBugReport);

					// Create activity event
					int userId = Int32.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
					var commentActivity = new ActivityBugReport(DateTime.Now, currentProjectId, ActivityMessage.BugReportPosted, userId, addedReport.BugReportId);
					activityRepository.Add(commentActivity);

					BugState newBugState = new BugState
					{
						Time = DateTime.Now,
						StateType = StateType.open,
						Author = HttpContext.User.Identity.Name,
						BugReportId = addedReport.BugReportId
					};
					BugState addedBugState = bugReportStatesRepository.Add(newBugState);
					
					// deal with subscriptions after bug states to prevent premature email updates
					if (model.Subscribe && !subscriptions.IsSubscribed(userId, addedReport.BugReportId))
					{
						// add to subscriptions in the repo
						userSubscriptionsRepository.AddSubscription(userId, addedReport.BugReportId);
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
		public IActionResult Edit(int bugReportId)
		{
			EditBugReportViewModel reportViewModel = new EditBugReportViewModel
			{
				BugReport = bugReportRepository.GetById(bugReportId),
				CurrentState = bugReportStatesRepository.GetLatestState(bugReportId).StateType
			};

			var currentProjectId = HttpContext.Session.GetInt32("currentProject");
			var currentProject = projectRepository.GetById(currentProjectId ?? 0);

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, new { ProjectId = currentProjectId, PersonReporting = reportViewModel.BugReport.PersonReporting}, "CanModifyReportPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				// --------------------- CONFIGURE BREADCRUMB NODES ----------------------------
				var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
				var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", currentProject.Name)
				{
					RouteValues = new { id = currentProjectId },
					Parent = projectsNode
				};
				var reportNode = new MvcBreadcrumbNode("ReportOverview", "BugReport", reportViewModel.BugReport.Title)
				{
					RouteValues = new { id = reportViewModel.BugReport.BugReportId },
					Parent = overviewNode
				};
				var editNode = new MvcBreadcrumbNode("Edit", "BugReport", "Edit")
				{
					Parent = reportNode
				};
				ViewData["BreadcrumbNode"] = editNode;
				// --------------------------------------------------------------------------------------------

				return View(reportViewModel);
			}

			return RedirectToAction("Overview", "Projects", new { id = currentProjectId});
		}

		[HttpPost]
		public IActionResult Edit(EditBugReportViewModel model)
		{
			if (ModelState.IsValid)
			{
				BugReport bugReport = bugReportRepository.GetById(model.BugReport.BugReportId);
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
				activityRepository.Add(activityEvent);

				BugState latestBugState = bugReportStatesRepository.GetLatestState(bugReport.BugReportId);
				if (!model.CurrentState.Equals(latestBugState.StateType))
				{
					BugState newBugState = new BugState
					{
						Time = DateTime.Now,
						StateType = model.CurrentState,
						Author = HttpContext.User.Identity.Name,
						BugReportId = bugReport.BugReportId
					};

					var createdBugState = bugReportStatesRepository.Add(newBugState);

					string bugReportUrl = Url.Action("ReportOverview", "BugReport", new { id = bugReport.BugReportId }, Request.Scheme);
					subscriptions.NotifyBugReportStateChanged(createdBugState, bugReportUrl);

					// Create activity event
					var stateActivityEvent = new ActivityBugReportStateChange(DateTime.Now, currentProjectId.Value, ActivityMessage.BugReportStateChanged, userId, bugReport.BugReportId, createdBugState.BugStateId, latestBugState.BugStateId);
					activityRepository.Add(stateActivityEvent);
				}

				return RedirectToAction("ReportOverview", new { id = bugReport.BugReportId});
			}

			return View();
		}

		public IActionResult Delete(int bugReportId)
		{
			int currentProjectId = (int)HttpContext.Session.GetInt32("currentProject");
			var bugReport = bugReportRepository.GetById(bugReportId);

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, new { ProjectId = currentProjectId, PersonReporting = bugReport.PersonReporting }, "CanModifyReportPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				bugReportRepository.Delete(bugReportId);
			}

			return RedirectToAction("Overview", "Projects", new { id = currentProjectId });
		}

		[HttpGet]
		public IActionResult AssignMember(int bugReportId)
		{
			int currentProjectId = (int)HttpContext.Session.GetInt32("currentProject");
			var bugReport = bugReportRepository.GetById(bugReportId);

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProjectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				// --------------------- CONFIGURE BREADCRUMB NODES ----------------------------
				var currentProject = projectRepository.GetById(currentProjectId);
				var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
				var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", currentProject.Name)
				{
					RouteValues = new { id = currentProjectId },
					Parent = projectsNode
				};
				var reportNode = new MvcBreadcrumbNode("ReportOverview", "BugReport", bugReport.Title)
				{
					RouteValues = new {id = bugReportId},
					Parent = overviewNode
				};
				var assignMembersNode = new MvcBreadcrumbNode("AssignMember", "BugReport", "Assign Members")
				{
					Parent = reportNode
				};
				ViewData["BreadcrumbNode"] = assignMembersNode;
				// --------------------------------------------------------------------------------------------

				AssignMemberViewModel viewModel = new AssignMemberViewModel()
				{
					BugReportId = bugReportId,
					ProjectId = currentProjectId,
					AssignedUsers = bugReportRepository.GetAssignedUsersForBugReport(bugReportId).ToList()
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

				bugReportRepository.AddUserAssignedToBugReport(assignedUserId, model.BugReportId);

				// Create activity event
				int userId = Int32.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
				var currentProjectId = HttpContext.Session.GetInt32("currentProject");
				var activityEvent = new ActivityBugReportAssigned(DateTime.Now, currentProjectId.Value, ActivityMessage.BugReportAssignedToUser, userId, model.BugReportId, assignedUserId);
				activityRepository.Add(activityEvent);

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

				bugReportRepository.DeleteUserAssignedToBugReport(Int32.Parse(user.Id), bugReportId);

				return RedirectToAction("AssignMember", new { bugReportId });
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public IActionResult ManageLinks(int bugReportId)
		{
			int currentProjectId = (int)HttpContext.Session.GetInt32("currentProject");
			var bugReport = bugReportRepository.GetById(bugReportId);

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProjectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				// --------------------- CONFIGURE BREADCRUMB NODES ----------------------------
				var currentProject = projectRepository.GetById(currentProjectId);
				var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
				var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", currentProject.Name)
				{
					RouteValues = new { id = currentProjectId },
					Parent = projectsNode
				};
				var reportNode = new MvcBreadcrumbNode("ReportOverview", "BugReport", bugReport.Title)
				{
					RouteValues = new { id = bugReportId },
					Parent = overviewNode
				};
				var manageLinksNode = new MvcBreadcrumbNode("ManageLinks", "BugReport", "Manage Links")
				{
					Parent = reportNode
				};
				ViewData["BreadcrumbNode"] = manageLinksNode;
				// --------------------------------------------------------------------------------------------

				ManageLinksViewModel model = new ManageLinksViewModel {
					ProjectId = currentProjectId,
					BugReportId = bugReportId,
					LinkedReports = bugReportRepository.GetLinkedReports(bugReportId).ToList()
				};

				return View(model);
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public IActionResult LinkReports(LinkReportsViewModel model)
		{
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, model.ProjectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				var linkToReport = bugReportRepository.GetBugReportByLocalId(model.LinkToBugReportLocalId, model.ProjectId);
				bugReportRepository.AddBugReportLink(model.BugReportId, linkToReport.BugReportId);

				// Create activity event
				int userId = Int32.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
				var currentProjectId = HttpContext.Session.GetInt32("currentProject");
				var activityEvent = new ActivityBugReportLink(DateTime.Now, currentProjectId.Value, ActivityMessage.BugReportsLinked, userId, model.BugReportId, linkToReport.BugReportId);
				activityRepository.Add(activityEvent);

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

		public IActionResult ReportOverview(int id)
		{
			var currentProjectId = HttpContext.Session.GetInt32("currentProject");

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProjectId, "CanAccessProjectPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				HttpContext.Session.SetInt32("currentBugReport", id);
				BugReport bugReport = bugReportRepository.GetById(id);

				var bugStates = bugReportStatesRepository.GetAllById(bugReport.BugReportId).OrderByDescending(o => o.Time).ToList();

				var assignedMembers = bugReportRepository.GetAssignedUsersForBugReport(id)
					.Select(x => new string(x.UserName)).ToList();
				string assignedMembersDisplay = "";
				if (assignedMembers.Count > 0)
					assignedMembersDisplay = string.Join(", ", assignedMembers);
				else
					assignedMembersDisplay = "Unassigned";

				OverviewBugReportViewModel bugViewModel = new OverviewBugReportViewModel
				{
					BugReport = bugReport,
					Comments = commentRepository.GetAllById(bugReport.BugReportId).ToList(),
					BugStates = bugStates,
					Activities = activityRepository.GetBugReportActivities(bugReport.BugReportId).ToList(),
					CurrentState = bugStates[0].StateType,
					AssignedMembersDisplay = assignedMembersDisplay
				};

				// generate activity messages
				var applicationLinkGenerator = new ApplicationLinkGenerator(httpContextAccessor, linkGenerator);
				var activityMessageBuilder = new ActivityMessageBuilder(applicationLinkGenerator, userManager, projectRepository, 
					bugReportRepository, milestoneRepository, bugReportStatesRepository);
				activityMessageBuilder.GenerateMessages(bugViewModel.Activities);

				int userId = Int32.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
				if (subscriptions.IsSubscribed(userId, bugViewModel.BugReport.BugReportId))
				{
					bugViewModel.DisableSubscribeButton = true;
				}
				var adminAuthorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProjectId, "ProjectAdministratorPolicy");
				if (adminAuthorizationResult.IsCompletedSuccessfully && adminAuthorizationResult.Result.Succeeded)
				{
					bugViewModel.DisableAssignMembersButton = false;
				}

				var currentProject = projectRepository.GetById(currentProjectId ?? 0);

				// --------------------- CONFIGURE BREADCRUMB NODES ----------------------------
				var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
				var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", currentProject.Name)
				{
					RouteValues = new { id = currentProjectId },
					Parent = projectsNode
				};
				var reportNode = new MvcBreadcrumbNode("CreateReport", "BugReport", bugReport.Title)
				{
					Parent = overviewNode
				};
				ViewData["BreadcrumbNode"] = reportNode;
				// --------------------------------------------------------------------------------------------

				return View(bugViewModel);
			}

			return RedirectToAction("Index", "Home");
		}
	}
}
