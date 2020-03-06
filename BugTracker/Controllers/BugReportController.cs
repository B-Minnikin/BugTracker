﻿using BugTracker.Models;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartBreadcrumbs.Attributes;
using SmartBreadcrumbs.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
	[Authorize]
	public class BugReportController : Controller
	{
		private readonly ILogger<BugReportController> logger;
		private readonly IProjectRepository projectRepository;
		private readonly IAuthorizationService authorizationService;

		public BugReportController(ILogger<BugReportController> logger,
									        IProjectRepository projectRepository,
										  IAuthorizationService authorizationService)
		{
			this.logger = logger;
			this.projectRepository = projectRepository;
			this.authorizationService = authorizationService;
		}

		[HttpGet]
		public ViewResult CreateReport()
		{
			var currentProjectId = HttpContext.Session.GetInt32("currentProject");
			var currentProject = projectRepository.GetProjectById(currentProjectId ?? 0);

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
					BugReport addedReport = projectRepository.AddBugReport(newBugReport);

					BugState newBugState = new BugState
					{
						Time = DateTime.Now,
						StateType = StateType.open,
						Author = HttpContext.User.Identity.Name,
						BugReportId = addedReport.BugReportId
					};
					BugState addedBugState = projectRepository.CreateBugState(newBugState);

					return RedirectToAction("ReportOverview", new { id = addedReport.BugReportId });
				}
			}

			return View();
		}

		[HttpGet]
		public IActionResult Edit(int bugReportId)
		{
			EditBugReportViewModel reportViewModel = new EditBugReportViewModel
			{
				BugReport = projectRepository.GetBugReportById(bugReportId),
				CurrentState = projectRepository.GetLatestState(bugReportId).StateType
			};

			var currentProjectId = HttpContext.Session.GetInt32("currentProject");
			var currentProject = projectRepository.GetProjectById(currentProjectId ?? 0);

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, new { ProjectId = currentProjectId, PersonReporting = reportViewModel.BugReport.PersonReporting}, "CanEditReportPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
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

				return View(reportViewModel);
			}

			return RedirectToAction("Overview", "Projects", new { id = currentProjectId});
		}

		[HttpPost]
		public IActionResult Edit(EditBugReportViewModel model)
		{
			if (ModelState.IsValid)
			{
				BugReport bugReport = projectRepository.GetBugReportById(model.BugReport.BugReportId);
				bugReport.Title = model.BugReport.Title;
				bugReport.DetailsToReproduce = model.BugReport.DetailsToReproduce;
				bugReport.ProgramBehaviour = model.BugReport.ProgramBehaviour;
				bugReport.Severity = model.BugReport.Severity;
				bugReport.Importance = model.BugReport.Importance;
				bugReport.Hidden = model.BugReport.Hidden;
				bugReport.CreationTime = model.BugReport.CreationTime;

				_ = projectRepository.UpdateBugReport(bugReport);

				BugState latestBugState = projectRepository.GetLatestState(bugReport.BugReportId);
				if (!model.CurrentState.Equals(latestBugState.StateType))
				{
					BugState newBugState = new BugState
					{
						Time = DateTime.Now,
						StateType = model.CurrentState,
						Author = "User", // to implement
						BugReportId = bugReport.BugReportId
					};

					projectRepository.CreateBugState(newBugState);
				}

				return RedirectToAction("ReportOverview", new { id = bugReport.BugReportId});
			}

			return View();
		}

		public IActionResult Delete(int bugReportId)
		{
			int currentProjectId = (int)HttpContext.Session.GetInt32("currentProject");
			var bugReport = projectRepository.GetBugReportById(bugReportId);

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, new { ProjectId = currentProjectId, PersonReporting = bugReport.PersonReporting }, "CanEditReportPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				projectRepository.DeleteBugReport(bugReportId);
			}

			return RedirectToAction("Overview", "Projects", new { id = currentProjectId });
		}

		public ViewResult ReportOverview(int id)
		{
			HttpContext.Session.SetInt32("currentBugReport", id);
			BugReport bugReport = projectRepository.GetBugReportById(id);

			var bugStates = projectRepository.GetBugStates(bugReport.BugReportId).OrderByDescending(o => o.Time).ToList();

			OverviewBugReportViewModel bugViewModel = new OverviewBugReportViewModel
			{
				BugReport = bugReport,
				BugReportComments = projectRepository.GetBugReportComments(bugReport.BugReportId).ToList(),
				BugStates = bugStates,
				CurrentState = bugStates[0].StateType
			};

			var currentProjectId = HttpContext.Session.GetInt32("currentProject");
			var currentProject = projectRepository.GetProjectById(currentProjectId ?? 0);

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

			return View(bugViewModel);
		}
	}
}
