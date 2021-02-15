﻿using BugTracker.Models;
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

namespace BugTracker.Controllers
{
	[Authorize]
	public class MilestoneController : Controller
	{
		private readonly ILogger<MilestoneController> logger;
		private readonly IProjectRepository projectRepository;
		private readonly IAuthorizationService authorizationService;
		private readonly IHttpContextAccessor httpContextAccessor;

		public MilestoneController(ILogger<MilestoneController> logger, 
			IProjectRepository projectRepository,
			IAuthorizationService authorizationService,
			IHttpContextAccessor httpContextAccessor)
		{
			this.logger = logger;
			this.projectRepository = projectRepository;
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
				ProjectMilestones = projectRepository.GetAllMilestones(projectId).ToList()
			};

			return View(model);
		}

		[HttpGet]
		public IActionResult Overview(int milestoneId)
		{
			Milestone model = projectRepository.GetMilestoneById(milestoneId);
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

				return View(model);
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

					var createdMilestone = projectRepository.AddMilestone(newMilestone);

					// handle bug report ids
					foreach(var reportEntry in model.MilestoneBugReportEntries)
					{
						BugReport report = projectRepository.GetBugReportByLocalId(reportEntry.LocalBugReportId, newMilestone.ProjectId);
						
						projectRepository.AddMilestoneBugReport(createdMilestone.MilestoneId, report.BugReportId);
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
					Milestone = projectRepository.GetMilestoneById(milestoneId),
					ProjectId = currentProjectId,
					MilestoneBugReportEntries = projectRepository.GetMilestoneBugReportEntries(milestoneId).ToList()
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
					projectRepository.UpdateMilestone(model.Milestone);
					UpdateEditedMilestoneBugReports(model.Milestone, model.MilestoneBugReportEntries);

					return RedirectToAction("Overview", "Milestone", new { milestoneId = model.Milestone.MilestoneId });
				}

				return View(model);
			}

			return RedirectToAction("Index", "Home");
		}

		private void UpdateEditedMilestoneBugReports(Milestone milestone, List<MilestoneBugReportEntry> editedBugReports)
		{
			List<MilestoneBugReportEntry> existingBugReports = projectRepository.GetMilestoneBugReportEntries(milestone.MilestoneId).ToList();

			// if bug report only present in entries - add to repo
			IEnumerable<MilestoneBugReportEntry> toAdd = editedBugReports.Except(existingBugReports, new MilestoneBugReportEntryEqualityComparer());

			// if bug report missing from entries, but exists in repo - delete in repo
			IEnumerable<MilestoneBugReportEntry> toDelete = existingBugReports.Except(editedBugReports, new MilestoneBugReportEntryEqualityComparer());

			foreach (var entry in toAdd)
			{
				int bugReportId = projectRepository.GetBugReportByLocalId(entry.LocalBugReportId, milestone.ProjectId).BugReportId;
				projectRepository.AddMilestoneBugReport(milestone.MilestoneId, bugReportId);
			}

			foreach(var entry in toDelete)
			{
				int bugReportId = projectRepository.GetBugReportByLocalId(entry.LocalBugReportId, milestone.ProjectId).BugReportId;
				projectRepository.RemoveMilestoneBugReport(milestone.MilestoneId, bugReportId);
			}
		}

		public IActionResult Delete(int milestoneId)
		{
			var currentProjectId = HttpContext.Session.GetInt32("currentProject");
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProjectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				projectRepository.DeleteMilestone(milestoneId);

				return RedirectToAction("Milestones", "Milestone", new { projectId = currentProjectId});
			}

			return RedirectToAction("Index", "Home");
		}
	}
}
