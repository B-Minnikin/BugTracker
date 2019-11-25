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
		private readonly IProjectRepository projectRepository;

		public ProjectsController(ILogger<HomeController> logger,
									IProjectRepository projectRepository)
		{
			this._logger = logger;
			this.projectRepository = projectRepository;
		}

		public ViewResult Projects()
		{
			return View();
		}

		public ViewResult Overview()
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
