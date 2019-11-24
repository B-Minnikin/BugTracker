using BugTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
	public class ProjectsController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public ProjectsController(ILogger<HomeController> logger)									
		{
			this._logger = logger;
		}

		public ViewResult Projects()
		{
			return View();
		}

		[HttpGet]
		public ViewResult CreateProject()
		{
			return View();
		}

		[HttpPost]
		public ViewResult CreateProject(Project project)
		{
			return View();
		}
	}
}
