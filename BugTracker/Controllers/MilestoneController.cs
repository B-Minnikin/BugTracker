using BugTracker.Models;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartBreadcrumbs.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Helpers;
using System.Globalization;
using System.Security.Claims;
using BugTracker.Repository;

namespace BugTracker.Controllers
{
	[Authorize]
	public class MilestoneController : Controller
	{
		private readonly ILogger<MilestoneController> logger;
		private readonly IProjectRepository projectRepository;
		private readonly IMilestoneRepository milestoneRepository;
		private readonly IAuthorizationService authorizationService;
		private readonly IHttpContextAccessor httpContextAccessor;

		public MilestoneController(ILogger<MilestoneController> logger, 
			IProjectRepository projectRepository,
			IMilestoneRepository milestoneRepository,
			IAuthorizationService authorizationService,
			IHttpContextAccessor httpContextAccessor)
		{
			this.logger = logger;
			this.projectRepository = projectRepository;
			this.milestoneRepository = milestoneRepository;
			this.authorizationService = authorizationService;
			this.httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		public IActionResult Milestones(int projectId)
		{
			// --------------------- CONFIGURE BREADCRUMB NODES ----------------------------
			var currentProject = projectRepository.GetProjectById(projectId);
			var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
			var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", currentProject.Name)
			{
				RouteValues = new { id = projectId },
				Parent = projectsNode
			};
			var milestonesNode = new MvcBreadcrumbNode("Milestones", "Milestone", "Milestones")
			{
				RouteValues = new { projectId = projectId },
				Parent = overviewNode
			};
			ViewData["BreadcrumbNode"] = milestonesNode;
			// --------------------------------------------------------------------------------------------

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProject.ProjectId, "ProjectAdministratorPolicy");
			bool isAuthorized = authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded;

			MilestonesViewModel model = new MilestonesViewModel()
			{
				ProjectId = projectId,
				ShowNewButton = isAuthorized,
				ProjectMilestones = new List<MilestoneContainer>()
			};

			var milestones = milestoneRepository.GetAllById(projectId);
			foreach (var milestone in milestones)
			{
				var bugReportEntries = milestoneRepository.GetMilestoneBugReportEntries(milestone.MilestoneId).ToList();
				foreach (var bugReport in bugReportEntries) // <------- REFACTOR NEEDED
				{
					bugReport.CurrentState = projectRepository.GetLatestState(bugReport.BugReportId);
				}

				model.ProjectMilestones.Add(new MilestoneContainer
				{
					Milestone = milestone,
					MilestoneProgress = new MilestoneProgress(bugReportEntries)
				});
			}

			return View(model);
		}

		[HttpGet]
		public IActionResult Overview(int milestoneId)
		{
			Milestone model = milestoneRepository.GetById(milestoneId);
			var currentProject = projectRepository.GetProjectById(model.ProjectId);

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProject.ProjectId, "CanAccessProjectPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				// --------------------- CONFIGURE BREADCRUMB NODES ----------------------------
				var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
				var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", currentProject.Name)
				{
					RouteValues = new { id = currentProject.ProjectId },
					Parent = projectsNode
				};
				var milestonesNode = new MvcBreadcrumbNode("Milestones", "Milestone", "Milestones")
				{
					RouteValues = new { projectId = currentProject.ProjectId },
					Parent = overviewNode
				};
				var chosenMilestoneNode = new MvcBreadcrumbNode("Overview", "Milestone", model.Title)
				{
					Parent = milestonesNode
				};
				ViewData["BreadcrumbNode"] = chosenMilestoneNode;
				// --------------------------------------------------------------------------------------------

				//  Create view model
				var bugReports = GenerateBugReportEntries(milestoneId).ToList();

				MilestoneOverviewViewModel viewModel = new MilestoneOverviewViewModel
				{
					Milestone = model,
					MilestoneBugReportEntries = bugReports,

					ProjectMilestone = new MilestoneContainer
					{
						Milestone = model,
						MilestoneProgress = new MilestoneProgress(bugReports)
					}
				};

				return View(viewModel);
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public IActionResult New(int projectId)
		{
			var currentProject = projectRepository.GetProjectById(projectId);

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProject.ProjectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				// --------------------- CONFIGURE BREADCRUMB NODES ----------------------------
				var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
				var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", currentProject.Name)
				{
					RouteValues = new { id = currentProject.ProjectId },
					Parent = projectsNode
				};
				var milestonesNode = new MvcBreadcrumbNode("Milestones", "Milestone", "Milestones")
				{
					RouteValues = new { projectId = currentProject.ProjectId },
					Parent = overviewNode
				};
				var newMilestoneNode = new MvcBreadcrumbNode("New", "Milestone", "New")
				{
					Parent = milestonesNode
				};
				ViewData["BreadcrumbNode"] = newMilestoneNode;
				// --------------------------------------------------------------------------------------------

				NewMilestoneViewModel model = new NewMilestoneViewModel()
				{
					ProjectId = projectId
				};

				return View(model);
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public IActionResult New(NewMilestoneViewModel model)
		{
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, model.ProjectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				if (ModelState.IsValid)
				{
					var newMilestone = new Milestone()
					{
						ProjectId = model.ProjectId,
						Title = model.Title,
						Description = model.Description,
						CreationTime = model.CreationTime,
						DueDate = model.DueDate
					};

					var createdMilestone = milestoneRepository.Add(newMilestone);

					// Create activity event
					int userId = Int32.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
					var milestoneActivityEvent = new ActivityMilestone(DateTime.Now, createdMilestone.ProjectId, ActivityMessage.MilestonePosted, userId, createdMilestone.MilestoneId);
					projectRepository.AddActivity(milestoneActivityEvent);

					// handle bug report ids
					foreach (var reportEntry in model.MilestoneBugReportEntries)
					{
						BugReport report = projectRepository.GetBugReportByLocalId(reportEntry.LocalBugReportId, newMilestone.ProjectId);
						
						milestoneRepository.AddMilestoneBugReport(createdMilestone.MilestoneId, report.BugReportId);

						// Create activity event
						var currentProjectId = HttpContext.Session.GetInt32("currentProject");
						var bugReportActivityEvent = new ActivityMilestone(DateTime.Now, currentProjectId.Value, ActivityMessage.MilestonePosted, userId, createdMilestone.MilestoneId);
						projectRepository.AddActivity(bugReportActivityEvent);
					}

					return RedirectToAction("Overview", "Milestone", new { milestoneId = createdMilestone.MilestoneId });
				}

				logger.LogWarning($"Invalid Milestone model state");
				return View(model);
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public IActionResult Edit(int milestoneId)
		{
			var currentProjectId = (int)HttpContext.Session.GetInt32("currentProject");
			var currentProject = projectRepository.GetProjectById(currentProjectId);
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProjectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				// --------------------- CONFIGURE BREADCRUMB NODES ----------------------------
				var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
				var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", currentProject.Name)
				{
					RouteValues = new { id = currentProject.ProjectId },
					Parent = projectsNode
				};
				var milestonesNode = new MvcBreadcrumbNode("Milestones", "Milestone", "Milestones")
				{
					RouteValues = new { projectId = currentProject.ProjectId },
					Parent = overviewNode
				};
				var editMilestoneNode = new MvcBreadcrumbNode("Edit", "Milestone", "Edit")
				{
					Parent = milestonesNode
				};
				ViewData["BreadcrumbNode"] = editMilestoneNode;
				// --------------------------------------------------------------------------------------------

				var viewModel = new EditMilestoneViewModel
				{
					Milestone = milestoneRepository.GetById(milestoneId),
					ProjectId = currentProjectId,
					MilestoneBugReportEntries = GenerateBugReportEntries(milestoneId).ToList()
				};

				return View(viewModel);
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public IActionResult Edit(EditMilestoneViewModel model)
		{
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, model.ProjectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				if (ModelState.IsValid)
				{
					milestoneRepository.Update(model.Milestone);
					UpdateEditedMilestoneBugReports(model.Milestone, model.MilestoneBugReportEntries);

					// Create activity event
					int userId = Int32.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
					var currentProjectId = HttpContext.Session.GetInt32("currentProject");
					var activityEvent = new ActivityMilestone(DateTime.Now, currentProjectId.Value, ActivityMessage.MilestoneEdited, userId, model.Milestone.MilestoneId);
					projectRepository.AddActivity(activityEvent);

					return RedirectToAction("Overview", "Milestone", new { milestoneId = model.Milestone.MilestoneId });
				}

				return View(model);
			}

			return RedirectToAction("Index", "Home");
		}

		private void UpdateEditedMilestoneBugReports(Milestone milestone, List<MilestoneBugReportEntry> editedBugReports)
		{
			List<MilestoneBugReportEntry> existingBugReports = milestoneRepository.GetMilestoneBugReportEntries(milestone.MilestoneId).ToList();

			// if bug report only present in entries - add to repo
			IEnumerable<MilestoneBugReportEntry> toAdd = editedBugReports.Except(existingBugReports, new MilestoneBugReportEntryEqualityComparer());

			// if bug report missing from entries, but exists in repo - delete in repo
			IEnumerable<MilestoneBugReportEntry> toDelete = existingBugReports.Except(editedBugReports, new MilestoneBugReportEntryEqualityComparer());

			// preparation data for creating activity events
			int userId = Int32.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
			var currentProjectId = HttpContext.Session.GetInt32("currentProject");

			foreach (var entry in toAdd)
			{
				int bugReportId = projectRepository.GetBugReportByLocalId(entry.LocalBugReportId, milestone.ProjectId).BugReportId;
				milestoneRepository.AddMilestoneBugReport(milestone.MilestoneId, bugReportId);

				// Create activity event
				var activityEvent = new ActivityMilestoneBugReport(DateTime.Now, currentProjectId.Value, ActivityMessage.BugReportAddedToMilestone, userId, milestone.MilestoneId, entry.BugReportId);
				projectRepository.AddActivity(activityEvent);
			}

			foreach(var entry in toDelete)
			{
				int bugReportId = projectRepository.GetBugReportByLocalId(entry.LocalBugReportId, milestone.ProjectId).BugReportId;
				milestoneRepository.RemoveMilestoneBugReport(milestone.MilestoneId, bugReportId);

				// Create activity event
				var activityEvent = new ActivityMilestoneBugReport(DateTime.Now, currentProjectId.Value, ActivityMessage.BugReportRemovedFromMilestone, userId, milestone.MilestoneId, entry.BugReportId);
				projectRepository.AddActivity(activityEvent);
			}
		}

		public IActionResult Delete(int milestoneId)
		{
			var currentProjectId = HttpContext.Session.GetInt32("currentProject");
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProjectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				milestoneRepository.Delete(milestoneId);

				return RedirectToAction("Milestones", "Milestone", new { projectId = currentProjectId});
			}

			return RedirectToAction("Index", "Home");
		}

		private IEnumerable<MilestoneBugReportEntry> GenerateBugReportEntries(int milestoneId)
		{
			Milestone milestone = milestoneRepository.GetById(milestoneId);
			IEnumerable<MilestoneBugReportEntry> entries = milestoneRepository.GetMilestoneBugReportEntries(milestoneId);

			foreach(var entry in entries)
			{
				entry.BugReportId = projectRepository.GetBugReportByLocalId(entry.LocalBugReportId, milestone.ProjectId).BugReportId;
				entry.Url = Url.Action("ReportOverview", "BugReport", new { id = entry.BugReportId });
				entry.CurrentState = projectRepository.GetLatestState(entry.BugReportId);
			}

			return entries;
		}

		private MilestoneBugReportEntry GenerateBugReportUrl(MilestoneBugReportEntry entry)
		{
			int currentProjectId = (int)HttpContext.Session.GetInt32("currentProject");
			int bugReportId = projectRepository.GetBugReportByLocalId(entry.LocalBugReportId, currentProjectId).BugReportId;
			return GenerateBugReportUrl(entry, bugReportId);
		}

		private MilestoneBugReportEntry GenerateBugReportUrl(MilestoneBugReportEntry entry, int bugReportId)
		{
			if (entry.Url == null)
			{
				entry.Url = Url.Action("ReportOverview", "BugReport", new { id = bugReportId });
			}
			return entry;
		}
	}
}
