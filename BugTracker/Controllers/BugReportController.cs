using BugTracker.Models;
using BugTracker.ViewModels;
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

		public BugReportController(ILogger<BugReportController> logger)
		{
			this.logger = logger;
		}

		[HttpGet]
		public ViewResult CreateReport()
		{
			return View();
		}

		[HttpPost]
		public ViewResult CreateReport(CreateBugReportViewModel model)
		{
			if (ModelState.IsValid)
			{
				BugReport newBugReport = new BugReport
				{
					Title = model.Title,
					ProgramBehaviour = model.ProgramBehaviour,
					DetailsToReproduce = model.DetailsToReproduce,
					ReportTime = DateTime.Now,
					Hidden = false, // to implement
					Severity = 1, // to implement
					Importance = 1, // to implement
					PersonReporting = "User" // to implement
									  // add BugState = open as default
				};

				// add bug report to current project
			}

			return View();
		}

		[HttpGet]
		public ViewResult EditReport()
		{
			return View();
		}

		[HttpPost]
		public ViewResult EditReport(BugReportViewModel model)
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
	}
}
