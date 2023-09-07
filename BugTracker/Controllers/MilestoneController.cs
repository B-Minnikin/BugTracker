using BugTracker.Models;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Helpers;
using System.Security.Claims;
using BugTracker.Database.Repository.Interfaces;

namespace BugTracker.Controllers;

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
		var isAuthorized = authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded;

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
			foreach (var bugReport in bugReportEntries.ToList()) // <------- REFACTOR NEEDED
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
		var model = await milestoneRepository .GetById(milestoneId);
		var currentProject = await projectRepository.GetById(model.ProjectId);

		var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProject.ProjectId, "CanAccessProjectPolicy");
		if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
		{
			ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.MilestoneOverview(currentProject, model.Title);

			//  Create view model
			var bugReports = await GenerateBugReportEntries(milestoneId);

			var viewModel = new MilestoneOverviewViewModel
			{
				Milestone = model,
				MilestoneBugReportEntries = bugReports.ToList(),

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
	public async Task<IActionResult> New(NewMilestoneViewModel viewModel)
	{
		var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, viewModel.ProjectId, "ProjectAdministratorPolicy");
		if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
		{
			if (ModelState.IsValid)
			{
				var newMilestone = new Milestone()
				{
					ProjectId = viewModel.ProjectId,
					Title = viewModel.Title,
					Description = viewModel.Description,
					CreationTime = viewModel.CreationTime,
					DueDate = viewModel.DueDate
				};

				var createdMilestone = await milestoneRepository.Add(newMilestone);

				// Create activity event
				var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				var milestoneActivityEvent = new ActivityMilestone(DateTime.Now, createdMilestone.ProjectId, ActivityMessage.MilestonePosted, userId, createdMilestone.MilestoneId);
				await activityRepository.Add(milestoneActivityEvent);

				// handle bug report ids
				foreach (var reportEntry in viewModel.MilestoneBugReportEntries)
				{
					var report = await bugReportRepository.GetBugReportByLocalId(reportEntry.LocalBugReportId, newMilestone.ProjectId);
					
					await milestoneRepository .AddMilestoneBugReport(createdMilestone.MilestoneId, report.BugReportId);

					// Create activity event
					var currentProjectId = HttpContext.Session.GetInt32("currentProject");
					var bugReportActivityEvent = new ActivityMilestone(DateTime.Now, currentProjectId.Value, ActivityMessage.MilestonePosted, userId, createdMilestone.MilestoneId);
					await activityRepository .Add(bugReportActivityEvent);
				}

				return RedirectToAction("Overview", "Milestone", new { milestoneId = createdMilestone.MilestoneId });
			}

			logger.LogWarning($"Invalid Milestone model state");
			return View(viewModel);
		}

		return RedirectToAction("Index", "Home");
	}

	[HttpGet]
	public async Task<IActionResult> Edit(int milestoneId)
	{
		var currentProjectId = httpContextAccessor.HttpContext?.Session.GetInt32("currentProject");
		if (!currentProjectId.HasValue) return BadRequest();
		
		var currentProject = await projectRepository .GetById(currentProjectId.Value);
		var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProjectId, "ProjectAdministratorPolicy");
		if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
		{
			ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.MilestoneEdit(currentProject);

			var milestoneBugReportEntries = await GenerateBugReportEntries(milestoneId);

			var viewModel = new EditMilestoneViewModel
			{
				Milestone = await milestoneRepository.GetById(milestoneId),
				ProjectId = currentProjectId.Value,
				MilestoneBugReportEntries = milestoneBugReportEntries.ToList()
			};

			return View(viewModel);
		}

		return RedirectToAction("Index", "Home");
	}

	[HttpPost]
	public async Task<IActionResult> Edit(EditMilestoneViewModel viewModel)
	{
		var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, viewModel.ProjectId, "ProjectAdministratorPolicy");
		if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
		{
			if (ModelState.IsValid)
			{
				await milestoneRepository.Update(viewModel.Milestone);
				await UpdateEditedMilestoneBugReports(viewModel.Milestone, viewModel.MilestoneBugReportEntries);

				// Create activity event
				var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				var currentProjectId = httpContextAccessor.HttpContext?.Session.GetInt32("currentProject");
				if (!currentProjectId.HasValue) return BadRequest();
				
				var activityEvent = new ActivityMilestone(DateTime.Now, currentProjectId.Value, ActivityMessage.MilestoneEdited, userId, viewModel.Milestone.MilestoneId);
				await activityRepository.Add(activityEvent);

				return RedirectToAction("Overview", "Milestone", new { milestoneId = viewModel.Milestone.MilestoneId });
			}

			return View(viewModel);
		}

		return RedirectToAction("Index", "Home");
	}

	private async Task UpdateEditedMilestoneBugReports(Milestone milestone, IReadOnlyCollection<MilestoneBugReportEntry> editedBugReports)
	{
		var existingBugReports = await milestoneRepository.GetMilestoneBugReportEntries(milestone.MilestoneId);
		var existingBugReportsList = existingBugReports.ToList();

		// if bug report only present in entries - add to repo
		var toAdd = editedBugReports.Except(existingBugReportsList, new MilestoneBugReportEntryEqualityComparer());

		// if bug report missing from entries, but exists in repo - delete in repo
		var toDelete = existingBugReportsList.Except(editedBugReports, new MilestoneBugReportEntryEqualityComparer());

		// preparation data for creating activity events
		var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		var currentProjectId = HttpContext.Session.GetInt32("currentProject");
		if (!currentProjectId.HasValue) throw new ArgumentException("Invalid project ID");

		foreach (var entry in toAdd)
		{
			var bugReport = await bugReportRepository.GetBugReportByLocalId(entry.LocalBugReportId, milestone.ProjectId);
			var bugReportId = bugReport.BugReportId;
			await milestoneRepository.AddMilestoneBugReport(milestone.MilestoneId, bugReportId);

			// Create activity event
			var activityEvent = new ActivityMilestoneBugReport(DateTime.Now, currentProjectId.Value, ActivityMessage.BugReportAddedToMilestone, userId, milestone.MilestoneId, entry.BugReportId);
			await activityRepository.Add(activityEvent);
		}

		foreach(var entry in toDelete)
		{
			var bugReport = await bugReportRepository.GetBugReportByLocalId(entry.LocalBugReportId, milestone.ProjectId);
			var bugReportId = bugReport.BugReportId;
			await milestoneRepository.RemoveMilestoneBugReport(milestone.MilestoneId, bugReportId);

			// Create activity event
			var activityEvent = new ActivityMilestoneBugReport(DateTime.Now, currentProjectId.Value, ActivityMessage.BugReportRemovedFromMilestone, userId, milestone.MilestoneId, entry.BugReportId);
			await activityRepository .Add(activityEvent);
		}
	}

	public async Task<IActionResult> Delete(int milestoneId)
	{
		var currentProjectId = HttpContext.Session.GetInt32("currentProject");
		var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProjectId, "ProjectAdministratorPolicy");
		if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
		{
			await milestoneRepository.Delete(milestoneId);

			return RedirectToAction("Milestones", "Milestone", new { projectId = currentProjectId});
		}

		return RedirectToAction("Index", "Home");
	}

	private async Task<IEnumerable<MilestoneBugReportEntry>> GenerateBugReportEntries(int milestoneId)
	{
		var milestone = await milestoneRepository.GetById(milestoneId);
		var entries = await milestoneRepository.GetMilestoneBugReportEntries(milestoneId);

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
		var currentProjectId = httpContextAccessor.HttpContext?.Session.GetInt32("currentProject");
		if (!currentProjectId.HasValue) throw new ArgumentException("Project ID is null");
		
		var bugReport = await bugReportRepository.GetBugReportByLocalId(entry.LocalBugReportId, currentProjectId.Value);
		var bugReportId = bugReport.BugReportId;
		return GenerateBugReportUrl(entry, bugReportId);
	}

	private MilestoneBugReportEntry GenerateBugReportUrl(MilestoneBugReportEntry entry, int bugReportId)
	{
		entry.Url ??= Url.Action("ReportOverview", "BugReport", new { id = bugReportId });
		
		return entry;
	}
}
