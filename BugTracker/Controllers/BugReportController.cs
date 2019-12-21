﻿using BugTracker.Models;
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
									  // add BugState = open as default
					ProjectId = (int)HttpContext.Session.GetInt32("currentProject") // get project ID from cookie
				};

				// add bug report to current project
				BugReport addedReport = projectRepository.AddBugReport(newBugReport);
				return RedirectToAction("ReportOverview", addedReport.BugReportId);
			}

			return View();
		}

		[HttpGet]
		public ViewResult Edit()
		{
			return View();
		}

		[HttpPost]
		public ViewResult Edit(BugReportViewModel model)
		{
			return View();
		}

		[HttpGet]
		public ViewResult CreateComment()
		{
			return View();
		}

		[HttpPost]
		public ViewResult CreateComment(CommentViewModel model)
		{
			return View();
		}

		public ViewResult ReportOverview(int id)
		{
			BugReport bugReport = projectRepository.GetBugReportById(id);

			OverviewBugReportViewModel bugViewModel = new OverviewBugReportViewModel
			{
				BugReport = bugReport,
				BugReportComments = projectRepository.GetBugReportComments(bugReport.BugReportId).ToList(),
				BugStates = projectRepository.GetBugStates(bugReport.BugReportId).ToList(),
				AttachmentPaths = projectRepository.GetAttachmentPaths(AttachmentParentType.BugReport, bugReport.BugReportId).ToList()
			};

			return View(bugViewModel);
		}
	}
}
