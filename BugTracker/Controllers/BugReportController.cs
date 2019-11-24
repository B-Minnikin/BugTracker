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
		public ViewResult CreateReport(BugReportViewModel model)
		{
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
	}
}
