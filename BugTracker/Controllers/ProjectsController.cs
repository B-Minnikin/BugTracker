using BugTracker.Models;
using BugTracker.ViewModels;
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

		[Breadcrumb("Projects", FromAction ="Index", FromController =typeof(HomeController))]
		public ViewResult Projects()
		{
			var model = projectRepository.GetAllProjects();

			return View(model);
		}

		[Breadcrumb("Overview", FromAction = "Projects", FromController = typeof(ProjectsController))]
		public ViewResult Overview(int id)
		{
			var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
			var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", "Overview test")
			{
				RouteValues = new { id = id},
				Parent = projectsNode
			};
			//ViewData ["BreadcrumbNode"] = projectsNode;
			ViewData["BreadcrumbNode"] = overviewNode;

			Project project = projectRepository.GetProjectById(id);
			HttpContext.Session.SetInt32("currentProject", id); // save project id to session

			OverviewProjectViewModel overviewProjectViewModel = new OverviewProjectViewModel()
			{
				Project = project,
				BugReports = projectRepository.GetAllBugReports(id).ToList(),
				CommentCountHandler = projectRepository.GetCommentCountById
			};

			// if project NULL -- redirect to error page !!

			return View(overviewProjectViewModel);
		}

		[HttpGet]
		[Breadcrumb("Create Project", FromAction = "Projects", FromController = typeof(ProjectsController))]
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

				Project addedProject = projectRepository.CreateProject(newProject);

				return RedirectToAction("Overview", new { id = addedProject.ProjectId });
			}

			return View();
		}

		public IActionResult DeleteProject(int id)
		{
			projectRepository.DeleteProject(id);

			return RedirectToAction("Projects");
		}
	}
}
