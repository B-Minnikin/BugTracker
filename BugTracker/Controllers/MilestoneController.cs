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
using BugTracker.Repository.Interfaces;

namespace BugTracker.Controllers
{
	[Authorize]
	public class MilestoneController : Controller
	{
		private readonly ILogger<MilestoneController> logger;
		private readonly IProjectRepository projectRepository;
		private readonly IMilestoneRepository milestoneRepository;
		private readonly IBugReportRepository bugReportRepository;
		private readonly IBugReportStatesRepository bugReportStatesRepository;
		private readonly IActivityRepository activityRepository;
		private readonly IAuthorizationService authorizationService;
		private readonly IHttpContextAccessor httpContextAccessor;

		public MilestoneController(ILogger<MilestoneController> logger, 
			IProjectRepository projectRepository,
			IMilestoneRepository milestoneRepository,
			IBugReportRepository bugReportRepository,
			IBugReportStatesRepository bugReportStatesRepository,
			IActivityRepository activityRepository,
			IAuthorizationService authorizationService,
			IHttpContextAccessor httpContextAccessor)
		{
			this.logger = logger;
			this.projectRepository = projectRepository;
			this.milestoneRepository = milestoneRepository;
			this.bugReportRepository = bugReportRepository;
			this.bugReportStatesRepository = bugReportStatesRepository;
			this.activityRepository = activityRepository;
			this.authorizationService = authorizationService;
			this.httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		public async Task<IActionResult> Milestones(int projectId)
		{
			var currentProject = await projectRepository.GetById(projectId);
			ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.Milestones(currentProject);

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProject.ProjectId, "ProjectAdministratorPolicy");
			bool isAuthorized = authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded;

			MilestonesViewModel model = new MilestonesViewModel()
			{
				ProjectId = projectId,
				ShowNewButton = isAuthorized,
				ProjectMilestones = new List<MilestoneContainer>()
			};

			var milestones = await milestoneRepository.GetAllById(projectId);
			foreach (var milestone in milestones)
			{
				var bugReportEntries = await milestoneRepository.GetMilestoneBugReportEntries(milestone.MilestoneId);
				foreach (var bugReport in bugReportEntries) // <------- REFACTOR NEEDED
				{
					bugReport.CurrentState = await bugReportStatesRepository.GetLatestState(bugReport.BugReportId);
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
		public async Task<IActionResult> Overview(int milestoneId)
		{
			Milestone model = await milestoneRepository .GetById(milestoneId);
			var currentProject = await projectRepository.GetById(model.ProjectId);

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProject.ProjectId, "CanAccessProjectPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.MilestoneOverview(currentProject, model.Title);

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
		public async Task<IActionResult> New(int projectId)
		{
			var currentProject = await projectRepository.GetById(projectId);

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProject.ProjectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.MilestoneCreate(currentProject);

				NewMilestoneViewModel model = new NewMilestoneViewModel()
				{
					ProjectId = projectId
				};

				return View(model);
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public async Task<IActionResult> New(NewMilestoneViewModel model)
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

					var createdMilestone = await milestoneRepository.Add(newMilestone);

					// Create activity event
					int userId = Int32.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
					var milestoneActivityEvent = new ActivityMilestone(DateTime.Now, createdMilestone.ProjectId, ActivityMessage.MilestonePosted, userId, createdMilestone.MilestoneId);
					await activityRepository.Add(milestoneActivityEvent);

					// handle bug report ids
					foreach (var reportEntry in model.MilestoneBugReportEntries)
					{
						BugReport report = await bugReportRepository.GetBugReportByLocalId(reportEntry.LocalBugReportId, newMilestone.ProjectId);
						
						await milestoneRepository .AddMilestoneBugReport(createdMilestone.MilestoneId, report.BugReportId);

						// Create activity event
						var currentProjectId = HttpContext.Session.GetInt32("currentProject");
						var bugReportActivityEvent = new ActivityMilestone(DateTime.Now, currentProjectId.Value, ActivityMessage.MilestonePosted, userId, createdMilestone.MilestoneId);
						await activityRepository .Add(bugReportActivityEvent);
					}

					return RedirectToAction("Overview", "Milestone", new { milestoneId = createdMilestone.MilestoneId });
				}

				logger.LogWarning($"Invalid Milestone model state");
				return View(model);
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public async Task<IActionResult> Edit(int milestoneId)
		{
			var currentProjectId = (int)HttpContext.Session.GetInt32("currentProject");
			var currentProject = await projectRepository .GetById(currentProjectId);
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProjectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.MilestoneEdit(currentProject);

				var viewModel = new EditMilestoneViewModel
				{
					Milestone = await milestoneRepository .GetById(milestoneId),
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
					activityRepository.Add(activityEvent);

					return RedirectToAction("Overview", "Milestone", new { milestoneId = model.Milestone.MilestoneId });
				}

				return View(model);
			}

			return RedirectToAction("Index", "Home");
		}

		private async Task UpdateEditedMilestoneBugReports(Milestone milestone, List<MilestoneBugReportEntry> editedBugReports)
		{
			List<MilestoneBugReportEntry> existingBugReports = await milestoneRepository.GetMilestoneBugReportEntries(milestone.MilestoneId).ToList();

			// if bug report only present in entries - add to repo
			IEnumerable<MilestoneBugReportEntry> toAdd = editedBugReports.Except(existingBugReports, new MilestoneBugReportEntryEqualityComparer());

			// if bug report missing from entries, but exists in repo - delete in repo
			IEnumerable<MilestoneBugReportEntry> toDelete = existingBugReports.Except(editedBugReports, new MilestoneBugReportEntryEqualityComparer());

			// preparation data for creating activity events
			int userId = Int32.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
			var currentProjectId = HttpContext.Session.GetInt32("currentProject");

			foreach (var entry in toAdd)
			{
				var bugReport = await bugReportRepository.GetBugReportByLocalId(entry.LocalBugReportId, milestone.ProjectId);
				int bugReportId = bugReport.BugReportId;
				await milestoneRepository.AddMilestoneBugReport(milestone.MilestoneId, bugReportId);

				// Create activity event
				var activityEvent = new ActivityMilestoneBugReport(DateTime.Now, currentProjectId.Value, ActivityMessage.BugReportAddedToMilestone, userId, milestone.MilestoneId, entry.BugReportId);
				await activityRepository.Add(activityEvent);
			}

			foreach(var entry in toDelete)
			{
				var bugReport = await bugReportRepository.GetBugReportByLocalId(entry.LocalBugReportId, milestone.ProjectId);
				int bugReportId = bugReport.BugReportId;
				await milestoneRepository.RemoveMilestoneBugReport(milestone.MilestoneId, bugReportId);

				// Create activity event
				var activityEvent = new ActivityMilestoneBugReport(DateTime.Now, currentProjectId.Value, ActivityMessage.BugReportRemovedFromMilestone, userId, milestone.MilestoneId, entry.BugReportId);
				await activityRepository .Add(activityEvent);
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

		private async Task<IEnumerable<MilestoneBugReportEntry>> GenerateBugReportEntries(int milestoneId)
		{
			Milestone milestone = await milestoneRepository.GetById(milestoneId);
			IEnumerable<MilestoneBugReportEntry> entries = await milestoneRepository.GetMilestoneBugReportEntries(milestoneId);

			foreach(var entry in entries)
			{
				var bugReport = await bugReportRepository.GetBugReportByLocalId(entry.LocalBugReportId, milestone.ProjectId);
				entry.BugReportId = bugReport.BugReportId;
				entry.Url = Url.Action("ReportOverview", "BugReport", new { id = entry.BugReportId });
				entry.CurrentState = await bugReportStatesRepository.GetLatestState(entry.BugReportId);
			}

			return entries;
		}

		private async Task<MilestoneBugReportEntry> GenerateBugReportUrl(MilestoneBugReportEntry entry)
		{
			int currentProjectId = (int)HttpContext.Session.GetInt32("currentProject");
			var bugReport = await bugReportRepository.GetBugReportByLocalId(entry.LocalBugReportId, currentProjectId);
			int bugReportId = bugReport.BugReportId;
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
