﻿using BugTracker.Helpers;
using BugTracker.Models;
using BugTracker.Models.Authorization;
using BugTracker.Models.ProjectInvitation;
using BugTracker.Repository;
using BugTracker.Repository.Interfaces;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartBreadcrumbs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
	public class ProjectsController : Controller
	{
		private readonly ILogger<ProjectsController> logger;
		private readonly IProjectRepository projectRepository;
		private readonly IBugReportRepository bugReportRepository;
		private readonly IActivityRepository activityRepository;
		private readonly IAuthorizationService authorizationService;
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly IProjectInviter projectInviter;
		private readonly ApplicationUserManager userManager;
		private readonly IConfiguration configuration;

		public ProjectsController(ILogger<ProjectsController> logger,
									IProjectRepository projectRepository,
									IBugReportRepository bugReportRepository,
									IActivityRepository activityRepository,
									IAuthorizationService authorizationService,
									IHttpContextAccessor httpContextAccessor,
									IProjectInviter projectInvitation,
									ApplicationUserManager userManager,
									IConfiguration configuration)
		{
			this.logger = logger;
			this.projectRepository = projectRepository;
			this.bugReportRepository = bugReportRepository;
			this.activityRepository = activityRepository;
			this.authorizationService = authorizationService;
			this.httpContextAccessor = httpContextAccessor;
			this.projectInviter = projectInvitation;
			this.userManager = userManager;
			this.configuration = configuration;
		}

		[Breadcrumb("Projects", FromAction ="Index", FromController =typeof(HomeController))]
		public async Task<ViewResult> Projects()
		{
			var model = await projectRepository.GetAll();
				model = model.Where(project =>
					{
						Task<AuthorizationResult> authorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User, project.ProjectId, "CanAccessProjectPolicy");
						if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
						{
							return true;
						}
						else { return false; }
					}
				);

			return View(model);
		}

		public async Task<IActionResult> Overview(int projectId)
		{
			if(projectId < 1)
			{
				return BadRequest();
			}

			var authorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User, projectId, "CanAccessProjectPolicy");
			if(authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				Project project = null;

				try
				{
					project = await projectRepository.GetById(projectId);
				}
				catch (InvalidOperationException ex)
				{
					logger.LogError(ex.Message);
					return NotFound();
				}

				httpContextAccessor.HttpContext.Session.SetInt32("currentProject", projectId); // save project id to session

				IEnumerable<BugReport> bugReports = await bugReportRepository.GetAllById(projectId);

				OverviewProjectViewModel overviewProjectViewModel = new OverviewProjectViewModel()
				{
					Project = project,
					BugReports = bugReports.ToList(),
					CommentCountHandler = bugReportRepository.GetCommentCountById
				};

				ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.ProjectOverview(project);

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

				Project addedProject = await projectRepository.Add(newProject);
				await bugReportRepository.AddLocalBugReportId(addedProject.ProjectId);

				// Create activity event
				int userId = Int32.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
				var activityEvent = new ActivityProject(DateTime.Now, addedProject.ProjectId, ActivityMessage.ProjectCreated, userId);
				await activityRepository.Add(activityEvent);

				// Add the user who created the project to its administrator role
				var user = await userManager.FindByIdAsync(userId.ToString());
				string connectionString = configuration.GetConnectionString("DBConnectionString");
				userManager.RegisterUserStore(new UserStore(connectionString));
				await userManager.AddToRoleAsync(user, "Administrator", addedProject.ProjectId);

				logger.LogInformation($"New project created. ID: {addedProject.ProjectId}, Name: {addedProject.Name}");

				return RedirectToAction("Overview", new { projectId = addedProject.ProjectId });
			}

			return BadRequest(ModelState);
		}

		public IActionResult DeleteProject(int projectId)
		{
			if (projectId < 1)
			{
				return BadRequest();
			}

			var authorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User, projectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				projectRepository.Delete(projectId);
				logger.LogInformation($"Project deleted. ID: {projectId}");
			}

			return RedirectToAction("Projects");
		}

		[HttpGet]
		public async Task<IActionResult> Edit(int projectId)
		{
			if(projectId < 1)
			{
				return BadRequest();
			}

			var authorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User, projectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				var project = await projectRepository.GetById(projectId);
				EditProjectViewModel projectViewModel = new EditProjectViewModel
				{
					Project = project
				};

				ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.ProjectEdit(project);
				
				return View(projectViewModel);
			}
			return RedirectToAction("Overview", new { projectId });
		}

		[HttpPost]
		public async Task<IActionResult> Edit(EditProjectViewModel model)
		{
			var authorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User, model.Project.ProjectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				if (ModelState.IsValid)
				{
					Project project = await projectRepository.GetById(model.Project.ProjectId);
					project.Name = model.Project.Name;
					project.Description = model.Project.Description;
					project.Hidden = model.Project.Hidden;
					project.LastUpdateTime = DateTime.Now;

					_ = projectRepository.Update(project);

					// Create activity event
					var claim = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
					int userId = Int32.Parse(claim.Value);
					var activityEvent = new ActivityProject(DateTime.Now, model.Project.ProjectId, ActivityMessage.ProjectEdited, userId);
					_ = await activityRepository.Add(activityEvent);

					return RedirectToAction("Overview", new { projectId = project.ProjectId });
				}
			}

			return View();
		}

		[HttpGet]
		public async Task<IActionResult> Invites(int projectId)
		{
			if(projectId < 1)
			{
				return BadRequest();
			}

			var authorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User, projectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				var project = await projectRepository.GetById(projectId);
				InvitesViewModel invitesViewModel = new InvitesViewModel
				{
					ProjectId = projectId
				};

				ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.ProjectInvites(project);
				
				return View(invitesViewModel);
			}

			return RedirectToAction("Overview", new { projectId });
		}

		[HttpPost]
		public async Task<IActionResult> Invites(InvitesViewModel model)
		{
			var authorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User, model.ProjectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				if (ModelState.IsValid)
				{
					ProjectInvitation invitation = new ProjectInvitation
					{
						EmailAddress = model.EmailAddress,
						Project = await projectRepository.GetById(model.ProjectId),
						ToUser = null,
						FromUser = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User)
					};

					await projectInviter.AddProjectInvitation(invitation);
					return RedirectToAction("Overview", "Projects", new { projectId = model.ProjectId });
				}
			}

			return View(model);
		}
	}
}
