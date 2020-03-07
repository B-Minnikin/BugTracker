using BugTracker.Models;
using BugTracker.Models.Authorization;
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
using System.Security.Claims;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
	public class ProjectsController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IProjectRepository projectRepository;
		private readonly IAuthorizationService authorizationService;
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly ApplicationUserManager userManager;

		public ProjectsController(ILogger<HomeController> logger,
									IProjectRepository projectRepository,
									IAuthorizationService authorizationService,
									IHttpContextAccessor httpContextAccessor)
		{
			this._logger = logger;
			this.projectRepository = projectRepository;
			this.authorizationService = authorizationService;
			this.httpContextAccessor = httpContextAccessor;
			this.userManager = new ApplicationUserManager();
		}

		[Breadcrumb("Projects", FromAction ="Index", FromController =typeof(HomeController))]
		public ViewResult Projects()
		{
			var model = projectRepository.GetAllProjects().Where(project =>
					{
						Task<AuthorizationResult> authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, project.ProjectId, "CanAccessProjectPolicy");
						if(authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
						{
							return true;
						}
						else { return false; }
					}
				);

			return View(model);
		}

		[Breadcrumb("Overview", FromAction = "Projects", FromController = typeof(ProjectsController))]
		public IActionResult Overview(int id)
		{
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, id, "CanAccessProjectPolicy");
			if(authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				Project project = projectRepository.GetProjectById(id);
				HttpContext.Session.SetInt32("currentProject", id); // save project id to session

				OverviewProjectViewModel overviewProjectViewModel = new OverviewProjectViewModel()
				{
					Project = project,
					BugReports = projectRepository.GetAllBugReports(id).ToList(),
					CommentCountHandler = projectRepository.GetCommentCountById
				};

				var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
				var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", project.Name)
				{
					RouteValues = new { id = id },
					Parent = projectsNode
				};
				ViewData["BreadcrumbNode"] = overviewNode;

				// if project NULL -- redirect to error page !!

				return View(overviewProjectViewModel);
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		[Authorize]
		[Breadcrumb("Create Project", FromAction = "Projects", FromController = typeof(ProjectsController))]
		public ViewResult CreateProject()
		{
			return View();
		}

		[HttpPost]
		[Authorize]
		public async Task<IActionResult> CreateProject(Project model)
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

				// Add the user who created the project to its administrator role
				string userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
				var user = await userManager.FindByIdAsync(userId);
				await userManager.AddToRoleAsync(user, "Administrator", addedProject.ProjectId);

				return RedirectToAction("Overview", new { id = addedProject.ProjectId });
			}

			return View();
		}

		public IActionResult DeleteProject(int id)
		{
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, id, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				projectRepository.DeleteProject(id);
			}

			return RedirectToAction("Projects");
		}
	}
}
