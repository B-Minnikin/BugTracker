using BugTracker.Helpers;
using BugTracker.Models;
using BugTracker.Models.Authorization;
using BugTracker.Models.ProjectInvitation;
using BugTracker.Repository.Interfaces;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
		private readonly ILogger<HomeController> _logger;
		private readonly IProjectRepository projectRepository;
		private readonly IBugReportRepository bugReportRepository;
		private readonly IActivityRepository activityRepository;
		private readonly IAuthorizationService authorizationService;
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly IProjectInviter projectInviter;
		private readonly ApplicationUserManager userManager;

		public ProjectsController(ILogger<HomeController> logger,
									IProjectRepository projectRepository,
									IBugReportRepository bugReportRepository,
									IActivityRepository activityRepository,
									IAuthorizationService authorizationService,
									IHttpContextAccessor httpContextAccessor,
									IProjectInviter projectInvitation,
									ApplicationUserManager userManager)
		{
			this._logger = logger;
			this.projectRepository = projectRepository;
			this.bugReportRepository = bugReportRepository;
			this.activityRepository = activityRepository;
			this.authorizationService = authorizationService;
			this.httpContextAccessor = httpContextAccessor;
			this.projectInviter = projectInvitation;
			this.userManager = userManager;
		}

		[Breadcrumb("Projects", FromAction ="Index", FromController =typeof(HomeController))]
		public async Task<ViewResult> Projects()
		{
			var model = await projectRepository.GetAll();
				model = model.Where(project =>
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

		public async Task<IActionResult> Overview(int id)
		{
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, id, "CanAccessProjectPolicy");
			if(authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				Project project = await projectRepository.GetById(id);
				HttpContext.Session.SetInt32("currentProject", id); // save project id to session

				IEnumerable<BugReport> bugReports = await bugReportRepository.GetAllById(id);

				OverviewProjectViewModel overviewProjectViewModel = new OverviewProjectViewModel()
				{
					Project = project,
					BugReports = bugReports.ToList(),
					CommentCountHandler = bugReportRepository.GetCommentCountById
				};

				ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.ProjectOverview(project);
				
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

				Project addedProject = await projectRepository .Add(newProject);
				await bugReportRepository.AddLocalBugReportId(addedProject.ProjectId);

				// Create activity event
				int userId = Int32.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
				var activityEvent = new ActivityProject(DateTime.Now, addedProject.ProjectId, ActivityMessage.ProjectCreated, userId);
				await activityRepository.Add(activityEvent);

				// Add the user who created the project to its administrator role
				var user = await userManager.FindByIdAsync(userId.ToString());
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
				projectRepository.Delete(id);
				_logger.LogInformation($"Project deleted. ID: {id}");
			}

			return RedirectToAction("Projects");
		}

		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, id, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				var project = await projectRepository.GetById(id);
				EditProjectViewModel projectViewModel = new EditProjectViewModel
				{
					Project = project
				};

				ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.ProjectEdit(project);
				
				return View(projectViewModel);
			}
			return RedirectToAction("Overview", new { id });
		}

		[HttpPost]
		public async Task<IActionResult> Edit(EditProjectViewModel model)
		{
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, model.Project.ProjectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				if (ModelState.IsValid)
				{
					Project project = await projectRepository .GetById(model.Project.ProjectId);
					project.Name = model.Project.Name;
					project.Description = model.Project.Description;
					project.Hidden = model.Project.Hidden;
					project.LastUpdateTime = DateTime.Now;

					_ = projectRepository.Update(project);

					// Create activity event
					int userId = Int32.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
					var activityEvent = new ActivityProject(DateTime.Now, model.Project.ProjectId, ActivityMessage.ProjectEdited, userId);
					await activityRepository .Add(activityEvent);

					return RedirectToAction("Overview", new { id = project.ProjectId });
				}
			}

			return View();
		}

		[HttpGet]
		public async Task<IActionResult> Invites(int id)
		{
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, id, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				var project = await projectRepository.GetById(id);
				InvitesViewModel invitesViewModel = new InvitesViewModel
				{
					ProjectId = id
				};

				ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.ProjectInvites(project);
				
				return View(invitesViewModel);
			}

			return RedirectToAction("Overview", id);
		}

		[HttpPost]
		public async Task<IActionResult> Invites(InvitesViewModel model)
		{
			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, model.ProjectId, "ProjectAdministratorPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				if (ModelState.IsValid)
				{
					ProjectInvitation invitation = new ProjectInvitation
					{
						EmailAddress = model.EmailAddress,
						Project = await projectRepository .GetById(model.ProjectId),
						ToUser = null,
						FromUser = await userManager.GetUserAsync(HttpContext.User)
					};

					await projectInviter.AddProjectInvitation(invitation);
					return RedirectToAction("Overview", "Projects", new { id = model.ProjectId });
				}
			}

			return View(model);
		}
	}
}
