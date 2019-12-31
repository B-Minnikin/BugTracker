using BugTracker.Models;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
	public class BugReportController : Controller
	{
		private readonly ILogger<BugReportController> logger;
		private readonly IProjectRepository projectRepository;

		public BugReportController(ILogger<BugReportController> logger,
									        IProjectRepository projectRepository)
		{
			this.logger = logger;
			this.projectRepository = projectRepository;
		}

		[HttpGet]
		public ViewResult CreateReport()
		{
			return View();
		}

		[HttpPost]
		public IActionResult CreateReport(CreateBugReportViewModel model)
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
					PersonReporting = "User", // to implement
					ProjectId = (int)HttpContext.Session.GetInt32("currentProject") // get project ID from cookie
				};

				// add bug report to current project
				BugReport addedReport = projectRepository.AddBugReport(newBugReport);

				BugState newBugState = new BugState
				{
					Time = DateTime.Now,
					StateType = StateType.open,
					Author = "User", // to implement
					BugReportId = addedReport.BugReportId
				};
				BugState addedBugState = projectRepository.CreateBugState(newBugState);

				return RedirectToAction("ReportOverview", new { id = addedReport.BugReportId });
			}

			return View();
		}

		[HttpGet]
		public ViewResult Edit(int bugReportId)
		{
			EditBugReportViewModel reportViewModel = new EditBugReportViewModel
			{
				BugReport = projectRepository.GetBugReportById(bugReportId),
				CurrentState = projectRepository.GetLatestBugState(bugReportId)
			};

			return View(reportViewModel);
		}

		[HttpPost]
		public IActionResult Edit(BugReport model)
		{
			if (ModelState.IsValid)
			{
				BugReport bugReport = projectRepository.GetBugReportById(model.BugReportId);
				bugReport.Title = model.Title;
				bugReport.DetailsToReproduce = model.DetailsToReproduce;
				bugReport.ProgramBehaviour = model.ProgramBehaviour;
				bugReport.Severity = model.Severity;
				bugReport.Importance = model.Importance;
				bugReport.Hidden = model.Hidden;
				bugReport.CreationTime = model.CreationTime;

				projectRepository.UpdateBugReport(bugReport);
				return RedirectToAction("ReportOverview", new { id = bugReport.BugReportId});
			}

			return View();
		}

		public IActionResult Delete(int bugReportId)
		{
			projectRepository.DeleteBugReport(bugReportId);
			int currentProjectId = (int)HttpContext.Session.GetInt32("currentProject");

			return RedirectToAction("Overview", "Projects", new { id = currentProjectId });
		}

		public ViewResult ReportOverview(int id)
		{
			BugReport bugReport = projectRepository.GetBugReportById(id);

			OverviewBugReportViewModel bugViewModel = new OverviewBugReportViewModel
			{
				BugReport = bugReport,
				BugReportComments = projectRepository.GetBugReportComments(bugReport.BugReportId).ToList(),
				BugStates = projectRepository.GetBugStates(bugReport.BugReportId).ToList(),
				//AttachmentPaths = projectRepository.GetAttachmentPaths(AttachmentParentType.BugReport, bugReport.BugReportId).ToList()
			};

			return View(bugViewModel);
		}
	}
}
