using BugTracker.Helpers;
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

namespace BugTracker.Controllers;

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

    [Breadcrumb("Projects", FromAction = "Index", FromController = typeof(HomeController))]
    public async Task<ViewResult> Projects()
    {
        var projects = await projectRepository.GetAll();
        var authorisedProjects = projects.ToAsyncEnumerable()
            .Where(async (project, cancellationToken) =>
            {
                if (cancellationToken.IsCancellationRequested) return false;

                var authorizationResult =
                    await authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext?.User, project.ProjectId,
                        "CanAccessProjectPolicy");
                return authorizationResult.Succeeded;
            });
                
        return View(authorisedProjects);
    }

    public async Task<IActionResult> Overview(int projectId)
    {
        try
        {
            var authorizationResult = await authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext?.User, projectId,
                "CanAccessProjectPolicy");
            if (authorizationResult.Succeeded) return RedirectToAction("Index", "Home");
        
            var project = await projectRepository.GetById(projectId);

            httpContextAccessor.HttpContext?.Session.SetInt32("currentProject", projectId); // Save project id to session

            var bugReports = await bugReportRepository.GetAllById(projectId);

            var overviewProjectViewModel = new OverviewProjectViewModel()
            {
                Project = project,
                BugReports = bugReports.ToList(),
                CommentCountHandler = bugReportRepository.GetCommentCountById
            };

            ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.ProjectOverview(project);

            return View(overviewProjectViewModel);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError("{Message}", ex.Message);
            return NotFound();
        }
        catch (Exception)
        {
            return BadRequest();
        }
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
    public async Task<IActionResult> CreateProject(Project project)
    {
        if (ModelState.IsValid)
        {
            var newProject = new Project
            {
                Name = project.Name,
                Description = project.Description,
                CreationTime = DateTime.Now,
                LastUpdateTime = DateTime.Now,
                Hidden = project.Hidden,
                BugReports = new List<BugReport>()
            };

            var addedProject = await projectRepository.Add(newProject);
            await bugReportRepository.AddLocalBugReportId(addedProject.ProjectId);

            // Create activity event
            var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var activityEvent = new ActivityProject(DateTime.Now, addedProject.ProjectId,
                ActivityMessage.ProjectCreated, userId);
            await activityRepository.Add(activityEvent);

            // Add the user who created the project to its administrator role
            var user = await userManager.FindByIdAsync(userId.ToString());
            var connectionString = configuration.GetConnectionString("DBConnectionString");
            userManager.RegisterUserStore(new UserStore(connectionString));
            await userManager.AddToRoleAsync(user, "Administrator", addedProject.ProjectId);

            logger.LogInformation($"New project created. ID: {addedProject.ProjectId}, Name: {addedProject.Name}");

            return RedirectToAction("Overview", new { projectId = addedProject.ProjectId });
        }

        return BadRequest(ModelState);
    }

    public async Task<IActionResult> DeleteProject(int projectId)
    {
        var authorizationResult = await authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User, projectId,
            "ProjectAdministratorPolicy");
        if (authorizationResult.Succeeded)
        {
            await projectRepository.Delete(projectId);
            logger.LogInformation($"Project deleted. ID: {projectId}");
        }

        return RedirectToAction("Projects");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int projectId)
    {
        if (projectId < 1)
        {
            return BadRequest();
        }

        var authorizationResult = await authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext?.User, projectId,
            "ProjectAdministratorPolicy");
        if (authorizationResult.Succeeded)
        {
            var project = await projectRepository.GetById(projectId);
            var projectViewModel = new EditProjectViewModel
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
        var authorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext?.User,
            model.Project.ProjectId, "ProjectAdministratorPolicy");
        if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
        {
            if (ModelState.IsValid)
            {
                var project = await projectRepository.GetById(model.Project.ProjectId);
                project.Name = model.Project.Name;
                project.Description = model.Project.Description;
                project.Hidden = model.Project.Hidden;
                project.LastUpdateTime = DateTime.Now;

                _ = projectRepository.Update(project);

                // Create activity event
                var claim = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
                if (claim == null) return RedirectToAction("Index", "Home");
                
                var userId = int.Parse(claim.Value);
                var activityEvent = new ActivityProject(DateTime.Now, model.Project.ProjectId,
                    ActivityMessage.ProjectEdited, userId);
                _ = await activityRepository.Add(activityEvent);

                return RedirectToAction("Overview", new { projectId = project.ProjectId });
            }
        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Invites(int projectId)
    {
        var authorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext?.User, projectId,
            "ProjectAdministratorPolicy");
        if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
        {
            var project = await projectRepository.GetById(projectId);
            var invitesViewModel = new InvitesViewModel
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
        var authorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext?.User,
            model.ProjectId, "ProjectAdministratorPolicy");
        if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
        {
            if (ModelState.IsValid)
            {
                var invitation = new ProjectInvitation
                {
                    EmailAddress = model.EmailAddress,
                    Project = await projectRepository.GetById(model.ProjectId),
                    ToUser = null,
                    FromUser = await userManager.GetUserAsync(httpContextAccessor.HttpContext?.User)
                };

                await projectInviter.AddProjectInvitation(invitation);
                return RedirectToAction("Overview", "Projects", new { projectId = model.ProjectId });
            }
        }

        return View(model);
    }
}
