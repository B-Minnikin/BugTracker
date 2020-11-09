﻿using BugTracker.Models;
using BugTracker.Models.Authorization;
using BugTracker.Models.ProjectInvitation;
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
		private readonly IProjectInvitation projectInvitation;
		private readonly ApplicationUserManager userManager;

		public ProjectsController(ILogger<HomeController> logger,
									IProjectRepository projectRepository,
									IAuthorizationService authorizationService,
									IHttpContextAccessor httpContextAccessor,
									IProjectInvitation projectInvitation)
		{
			this._logger = logger;
			this.projectRepository = projectRepository;
			this.authorizationService = authorizationService;
			this.httpContextAccessor = httpContextAccessor;
			this.projectInvitation = projectInvitation;
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

				// --------------------- CONFIGURE BREADCRUMB NODES ----------------------------
				var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
				var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", project.Name)
				{
					RouteValues = new { id },
					Parent = projectsNode
				};
				ViewData["BreadcrumbNode"] = overviewNode;
				// --------------------------------------------------------------------------------------------

				// if project NULL -- redirect to error page !!

				return View(overviewProjectViewModel);
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		[Authorize]
		[Breadcrumb("Create Project", FromAction = "Projects")]
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

				_logger.LogInformation($"New project created. ID: {addedProject.ProjectId}, Name: {addedProject.Name}");

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
				_logger.LogInformation($"Project deleted. ID: {id}");
			}

			return RedirectToAction("Projects");
		}

		[HttpGet]
		public IActionResult Edit(int id)
		{
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, id, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				var project = projectRepository.GetProjectById(id);
				EditProjectViewModel projectViewModel = new EditProjectViewModel
				{
					Project = project
				};

				// --------------------- CONFIGURE BREADCRUMB NODES ----------------------------
				var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
				var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", project.Name)
				{
					RouteValues = new { id },
					Parent = projectsNode
				};
				var editProjectNode = new MvcBreadcrumbNode("Edit", "Projects", "Edit")
				{
					Parent = overviewNode
				};
				ViewData["BreadcrumbNode"] = editProjectNode;
				// --------------------------------------------------------------------------------------------

				return View(projectViewModel);
			}
			return View();
		}

		[HttpPost]
		public IActionResult Edit(EditProjectViewModel model)
		{
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, model.Project.ProjectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				if (ModelState.IsValid)
				{
					Project project = projectRepository.GetProjectById(model.Project.ProjectId);
					project.Name = model.Project.Name;
					project.Description = model.Project.Description;
					project.Hidden = model.Project.Hidden;
					project.LastUpdateTime = DateTime.Now;

					_ = projectRepository.UpdateProject(project);

					return RedirectToAction("Overview", new { id = project.ProjectId });
				}
			}

			return View();
		}

		[HttpGet]
		public IActionResult Invites(int id)
		{
			var project = projectRepository.GetProjectById(id);
			InvitesViewModel invitesViewModel = new InvitesViewModel
			{
				ProjectId = id
			};

			// --------------------- CONFIGURE BREADCRUMB NODES ----------------------------
			var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
			var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", project.Name)
			{
				RouteValues = new { id },
				Parent = projectsNode
			};
			var invitesProjectNode = new MvcBreadcrumbNode("Invites", "Projects", "Invites")
			{
				Parent = overviewNode
			};
			ViewData["BreadcrumbNode"] = invitesProjectNode;
			// --------------------------------------------------------------------------------------------

			return View(invitesViewModel);
		}

		[HttpPost]
		public IActionResult Invites(InvitesViewModel model)
		{
			if(ModelState.IsValid)
			{
				projectInvitation.AddProjectInvitation(model.EmailAddress, model.ProjectId);
				return RedirectToAction("Overview", "Projects", new { id = model.ProjectId });
			}

			return View(model);
		}
	}
}
