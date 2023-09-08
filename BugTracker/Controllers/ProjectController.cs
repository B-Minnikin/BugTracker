using BugTracker.Helpers;
using BugTracker.Models;
using BugTracker.Models.Authorization;
using BugTracker.Models.ProjectInvitation;
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
using BugTracker.Database.Repository.Interfaces;

namespace BugTracker.Controllers;

public class ProjectController : Controller
{
	private readonly ILogger<ProjectController> logger;
	private readonly IProjectRepository projectRepository;
	private readonly IBugReportRepository bugReportRepository;
	private readonly IActivityRepository activityRepository;
	private readonly IAuthorizationService authorizationService;
	private readonly IHttpContextAccessor httpContextAccessor;
	private readonly IProjectInviter projectInviter;
	private readonly ApplicationUserManager userManager;

	public ProjectController(ILogger<ProjectController> logger,
								IProjectRepository projectRepository,
								IBugReportRepository bugReportRepository,
								IActivityRepository activityRepository,
								IAuthorizationService authorizationService,
								IHttpContextAccessor httpContextAccessor,
								IProjectInviter projectInvitation,
								ApplicationUserManager userManager)
	{
		this.logger = logger;
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
					var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, project.ProjectId, "CanAccessProjectPolicy");
					return authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded;
				}
			);

		return View(model);
	}

	public async Task<IActionResult> Overview(int projectId)
	{
		var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, projectId, "CanAccessProjectPolicy");
		if (!authorizationResult.IsCompletedSuccessfully || !authorizationResult.Result.Succeeded)
		{
			return RedirectToAction("Index", "Home");
		}
		
		var project = await projectRepository.GetById(projectId);
		if (project == null) return NotFound();
		HttpContext.Session.SetInt32("currentProject", projectId); // save project id to session

		var bugReports = await bugReportRepository.GetAllById(projectId);

		var viewModel = new OverviewProjectViewModel()
		{
			Project = project,
			BugReports = bugReports.ToList(),
			CommentCountHandler = bugReportRepository.GetCommentCountById
		};

		ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.ProjectOverview(project);

		return View(viewModel);
	}

	[HttpGet]
	[Authorize]
	[Breadcrumb("Create Project", FromAction = "Projects")]
	public ViewResult Create()
	{
		return View();
	}

	[HttpPost]
	[Authorize]
	public async Task<IActionResult> Create(Project model)
	{
		if (!ModelState.IsValid)
		{
			return View();
		}
		
		var project = new Project
		{
			Name = model.Name,
			Description = model.Description,
			CreationTime = DateTime.Now,
			LastUpdateTime = DateTime.Now,
			Hidden = model.Hidden,
			BugReports = new List<BugReport>()
		};

		_ = await projectRepository.Add(project);
		await bugReportRepository.AddLocalBugReportId(project.ProjectId);

		// Create activity event
		var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		
		var activityEvent = new ActivityProject(DateTime.Now, project.ProjectId, ActivityMessage.ProjectCreated, userId);
		await activityRepository.Add(activityEvent);

		// Add the user who created the project to its administrator role
		var user = await userManager.FindByIdAsync(userId);
		await userManager.AddToRoleAsync(user, "Administrator", project.ProjectId);

		logger.LogInformation($"New project created. ID: {project.ProjectId}, Name: {project.Name}");

		return RedirectToAction("Overview", new { id = project.ProjectId });
	}

	public async Task<IActionResult> Delete(int projectId)
	{
		var user = httpContextAccessor.HttpContext?.User;
		if (user is null) return BadRequest();
		
		var authorizationResult = authorizationService.AuthorizeAsync(user, projectId, "ProjectAdministratorPolicy");
		if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
		{
			await projectRepository.Delete(projectId);
			logger.LogInformation($"Project deleted. ID: {projectId}");
		}

		return RedirectToAction("Projects");
	}

	[HttpGet]
	public async Task<IActionResult> Edit(int projectId)
	{
		var user = httpContextAccessor.HttpContext?.User;
		if (user is null) return BadRequest();
		
		var authorizationResult = authorizationService.AuthorizeAsync(user, projectId, "ProjectAdministratorPolicy");
		if (!authorizationResult.IsCompletedSuccessfully || !authorizationResult.Result.Succeeded)
		{
			return RedirectToAction("Overview", new { projectId });
		}
		
		var project = await projectRepository.GetById(projectId);
		var viewModel = new EditProjectViewModel
		{
			Project = project
		};

		ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.ProjectEdit(project);
		
		return View(viewModel);
	}

	[HttpPost]
	public async Task<IActionResult> Edit(EditProjectViewModel viewModel)
	{
		var user = httpContextAccessor.HttpContext?.User;
		if (user is null) return BadRequest();
		
		var authorizationResult = authorizationService.AuthorizeAsync(user, viewModel.Project.ProjectId, "ProjectAdministratorPolicy");
		if (!authorizationResult.IsCompletedSuccessfully || !authorizationResult.Result.Succeeded)
		{
			return Forbid();
		}

		if (!ModelState.IsValid)
		{
			return View(viewModel);
		}
		
		var project = await projectRepository.GetById(viewModel.Project.ProjectId);
		if (project == null) return BadRequest();
		
		project.Name = viewModel.Project.Name;
		project.Description = viewModel.Project.Description;
		project.Hidden = viewModel.Project.Hidden;
		project.LastUpdateTime = DateTime.Now;

		_ = projectRepository.Update(project);

		// Create activity event
		var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		if (userId is null) return BadRequest();
		
		var activityEvent = new ActivityProject(DateTime.Now, viewModel.Project.ProjectId, ActivityMessage.ProjectEdited, userId);
		await activityRepository .Add(activityEvent);

		return RedirectToAction("Overview", new { id = project.ProjectId });
	}

	[HttpGet]
	public async Task<IActionResult> Invites(int projectId)
	{
		var user = httpContextAccessor.HttpContext?.User;
		if (user is null) return BadRequest();
		
		var authorizationResult = authorizationService.AuthorizeAsync(user, projectId, "ProjectAdministratorPolicy");
		if (!authorizationResult.IsCompletedSuccessfully || !authorizationResult.Result.Succeeded)
		{
			return RedirectToAction("Overview", projectId);
		}
		
		var project = await projectRepository.GetById(projectId);
		var viewModel = new InvitesViewModel
		{
			ProjectId = projectId
		};

		ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.ProjectInvites(project);
		
		return View(viewModel);
	}

	[HttpPost]
	public async Task<IActionResult> Invites(InvitesViewModel model)
	{
		var user = httpContextAccessor.HttpContext?.User;
		if (user is null) return BadRequest();
		
		var authorizationResult = authorizationService.AuthorizeAsync(user, model.ProjectId, "ProjectAdministratorPolicy");
		if (!authorizationResult.IsCompletedSuccessfully || !authorizationResult.Result.Succeeded)
		{
			return Forbid();
		}

		if (!ModelState.IsValid)
		{
			return View(model);
		}
		
		var invitation = new ProjectInvitation
		{
			EmailAddress = model.EmailAddress,
			Project = await projectRepository .GetById(model.ProjectId),
			ToUser = null,
			FromUser = await userManager.GetUserAsync(HttpContext.User)
		};

		await projectInviter.AddProjectInvitation(invitation);
		return RedirectToAction("Overview", "Project", new { id = model.ProjectId });
	}
}
