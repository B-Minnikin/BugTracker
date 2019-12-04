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
			var model = projectRepository.GetAllProjects();

			return View(model);
		}

		public ViewResult Overview(int id)
		{
			Project project = projectRepository.GetProject(id);

			OverviewProjectViewModel overviewProjectViewModel = new OverviewProjectViewModel()
			{
				Project = project
			};

			// if project NULL -- redirect to error page !!

			return View(overviewProjectViewModel);
		}

		[HttpGet]
		public ViewResult CreateProject()
		{
			return View();
		}

		[HttpPost]
		public IActionResult CreateProject(Project model)
		{
			if (ModelState.IsValid)
			{
				Project newProject = new Project
				{
					Name = model.Name,
					Description = model.Description,
					CreationTime = DateTime.Now,
					LastUpdateTime = DateTime.Now,
					Hidden = model.Hidden,
					BugReports = new List<BugReport>()
				};

				projectRepository.Add(newProject);

				return RedirectToAction("overview", newProject.Id);
			}

			return View();
		}

		public ViewResult DeleteProject()
		{
			return View();
		}
	}
}
