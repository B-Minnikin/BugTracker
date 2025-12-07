using BugTracker.Helpers;
using BugTracker.Models;
using BugTracker.Models.Authorization;
using BugTracker.Models.Subscription;
using BugTracker.Repository.Interfaces;
using BugTracker.Services;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BugTracker.Controllers;

[Authorize]
public class BugReportController : Controller
{
    private readonly ILogger<BugReportController> logger;
    private readonly IProjectRepository projectRepository;
    private readonly IMilestoneRepository milestoneRepository;
    private readonly IBugReportRepository bugReportRepository;
    private readonly IBugReportStatesRepository bugReportStatesRepository;
    private readonly IUserSubscriptionsRepository userSubscriptionsRepository;
    private readonly IActivityRepository activityRepository;
    private readonly ICommentRepository commentRepository;
    private readonly IAuthorizationService authorizationService;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ISubscriptions subscriptions;
    private readonly LinkGenerator linkGenerator;
    private readonly ApplicationLinkGenerator applicationLinkGenerator;
    private readonly ApplicationUserManager userManager;

    public BugReportController(ILogger<BugReportController> logger,
        IProjectRepository projectRepository,
        IMilestoneRepository milestoneRepository,
        IBugReportRepository bugReportRepository,
        IBugReportStatesRepository bugReportStatesRepository,
        IUserSubscriptionsRepository userSubscriptionsRepository,
        IActivityRepository activityRepository,
        ICommentRepository commentRepository,
        IAuthorizationService authorizationService,
        IHttpContextAccessor httpContextAccessor,
        ISubscriptions subscriptions,
        LinkGenerator linkGenerator,
        ApplicationUserManager userManager)
    {
        this.logger = logger;
        this.projectRepository = projectRepository;
        this.milestoneRepository = milestoneRepository;
        this.bugReportRepository = bugReportRepository;
        this.bugReportStatesRepository = bugReportStatesRepository;
        this.userSubscriptionsRepository = userSubscriptionsRepository;
        this.activityRepository = activityRepository;
        this.commentRepository = commentRepository;
        this.authorizationService = authorizationService;
        this.httpContextAccessor = httpContextAccessor;
        this.subscriptions = subscriptions;
        this.linkGenerator = linkGenerator;
        this.userManager = userManager;

        applicationLinkGenerator = new ApplicationLinkGenerator(httpContextAccessor, linkGenerator);
    }

    [HttpGet]
    public async Task<IActionResult> CreateReport()
    {
        var currentProjectId = httpContextAccessor.HttpContext?.Session.GetInt32("currentProject") ?? 0;
        if (currentProjectId < 1)
        {
            return NotFound();
        }

        if (httpContextAccessor.HttpContext?.User == null) return View("Error");

        var authorizationResult = await authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User,
            currentProjectId, "CanAccessProjectPolicy");

        if (!authorizationResult.Succeeded)
            return RedirectToAction("Overview", "Projects", new { projectId = currentProjectId });

        try
        {
            var currentProject = await projectRepository.GetById(currentProjectId);
            ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.BugReportCreate(currentProject);

            return View();
        }
        catch (Exception)
        {
            return View("Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateReport(CreateBugReportViewModel model)
    {
        if (model == null)
        {
            return BadRequest();
        }

        var currentProjectId = httpContextAccessor.HttpContext?.Session.GetInt32("currentProject") ?? 0;
        if (currentProjectId < 1)
        {
            return NotFound();
        }

        var authorizationResult = await authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext?.User,
            currentProjectId, "CanAccessProjectPolicy");
        if (authorizationResult.Succeeded)
        {
            if (ModelState.IsValid)
            {
                var newBugReport = new BugReport
                {
                    Title = model.Title,
                    ProgramBehaviour = model.ProgramBehaviour,
                    DetailsToReproduce = model.DetailsToReproduce,
                    CreationTime = DateTime.Now,
                    Hidden = model.Hidden,
                    Severity = model.Severity,
                    Importance = model.Importance,
                    PersonReporting = httpContextAccessor.HttpContext?.User.Identity?.Name,
                    ProjectId = currentProjectId
                };

                // add bug report to current project
                var addedReport = await bugReportRepository.Add(newBugReport);

                // Create activity event
                var userId =
                    Int32.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var commentActivity = new ActivityBugReport(DateTime.Now, currentProjectId,
                    ActivityMessage.BugReportPosted, userId, addedReport.BugReportId);
                await activityRepository.Add(commentActivity);

                var newBugState = new BugState
                {
                    Time = DateTime.Now,
                    StateType = StateType.Open,
                    Author = httpContextAccessor.HttpContext.User.Identity?.Name,
                    BugReportId = addedReport.BugReportId
                };
                _ = await bugReportStatesRepository.Add(newBugState);

                // Deal with subscriptions after bug states to prevent premature email updates
                if (model.Subscribe && !await subscriptions.IsSubscribed(userId, addedReport.BugReportId))
                {
                    // Add to subscriptions in the repo
                    await userSubscriptionsRepository.AddSubscription(userId, addedReport.BugReportId);
                }

                return RedirectToAction("ReportOverview", new { bugReportId = addedReport.BugReportId });
            }

            return View(model);
        }

        return RedirectToAction("Overview", "Projects", new { projectId = currentProjectId });
    }

    public async Task<IActionResult> Subscribe(int bugReportId)
    {
        if (bugReportId < 1)
        {
            return BadRequest();
        }

        var currentProjectId = httpContextAccessor.HttpContext?.Session.GetInt32("currentProject");
        if (currentProjectId < 1)
        {
            return NotFound();
        }

        var authorizationResult = await authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext?.User,
            currentProjectId, "CanAccessProjectPolicy");
        if (authorizationResult.Succeeded)
        {
            var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            await subscriptions.CreateSubscriptionIfNotSubscribed(userId, bugReportId);
        }

        return RedirectToAction("ReportOverview", new { bugReportId = bugReportId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int bugReportId)
    {
        if (bugReportId < 1)
        {
            return BadRequest();
        }

        var currentProjectId = httpContextAccessor.HttpContext?.Session.GetInt32("currentProject");
        if (currentProjectId == null)
        {
            return NotFound();
        }

        var currentProject = await projectRepository.GetById(currentProjectId.Value);

        var latestState = await bugReportStatesRepository.GetLatestState(bugReportId);
        var reportViewModel = new EditBugReportViewModel
        {
            BugReport = await bugReportRepository.GetById(bugReportId),
            CurrentState = latestState.StateType
        };

        var authorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User,
            new { ProjectId = currentProjectId, PersonReporting = reportViewModel.BugReport.PersonReporting },
            "CanModifyReportPolicy");
        if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
        {
            ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.BugReportEdit(currentProject, reportViewModel.BugReport);

            return View(reportViewModel);
        }

        return RedirectToAction("Overview", "Projects", new { id = currentProjectId });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditBugReportViewModel viewModel)
    {
        if (viewModel == null)
        {
            return BadRequest();
        }

        var currentProjectId = httpContextAccessor.HttpContext?.Session.GetInt32("currentProject");
        if (currentProjectId == null)
        {
            return NotFound();
        }

        var authorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User,
            new { ProjectId = currentProjectId, PersonReporting = viewModel.BugReport.PersonReporting },
            "CanModifyReportPolicy");
        if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
        {
            if (ModelState.IsValid)
            {
                var bugReport = await bugReportRepository.GetById(viewModel.BugReport.BugReportId);
                bugReport.Title = viewModel.BugReport.Title;
                bugReport.DetailsToReproduce = viewModel.BugReport.DetailsToReproduce;
                bugReport.ProgramBehaviour = viewModel.BugReport.ProgramBehaviour;
                bugReport.Severity = viewModel.BugReport.Severity;
                bugReport.Importance = viewModel.BugReport.Importance;
                bugReport.Hidden = viewModel.BugReport.Hidden;
                bugReport.CreationTime = viewModel.BugReport.CreationTime;

                _ = bugReportRepository.Update(bugReport);

                // Create activity event
                var userId =
                    int.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var activityEvent = new ActivityBugReport(DateTime.Now, currentProjectId.Value,
                    ActivityMessage.BugReportEdited, userId, bugReport.BugReportId);
                await activityRepository.Add(activityEvent);

                // if last and latest states don't match then notify and log
                var latestBugState = await bugReportStatesRepository.GetLatestState(bugReport.BugReportId);
                if (!viewModel.CurrentState.Equals(latestBugState.StateType))
                {
                    var newBugState = new BugState
                    {
                        Time = DateTime.Now,
                        StateType = viewModel.CurrentState,
                        Author = httpContextAccessor.HttpContext.User.Identity?.Name,
                        BugReportId = bugReport.BugReportId
                    };

                    var createdBugState = await bugReportStatesRepository.Add(newBugState);
                    await subscriptions.NotifyBugReportStateChanged(createdBugState, applicationLinkGenerator,
                        bugReport.BugReportId);

                    // Create activity event
                    var stateActivityEvent = new ActivityBugReportStateChange(DateTime.Now, currentProjectId.Value,
                        ActivityMessage.BugReportStateChanged, userId, bugReport.BugReportId,
                        createdBugState.BugStateId, latestBugState.BugStateId);
                    await activityRepository.Add(stateActivityEvent);
                }

                return RedirectToAction("ReportOverview", new { bugReportId = bugReport.BugReportId });
            }

            return View(viewModel);
        }

        return RedirectToAction("ReportOverview", new { bugReportId = viewModel.BugReport.BugReportId });
    }

    public async Task<IActionResult> Delete(int bugReportId)
    {
        var currentProjectId = httpContextAccessor.HttpContext?.Session.GetInt32("currentProject");
        if (currentProjectId == null)
        {
            return NotFound();
        }

        var bugReport = await bugReportRepository.GetById(bugReportId);
        if (bugReport == null)
        {
            return NotFound();
        }

        var authorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User,
            new { ProjectId = currentProjectId, bugReport.PersonReporting }, "CanModifyReportPolicy");
        if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
        {
            await bugReportRepository.Delete(bugReportId);
        }

        return RedirectToAction("Overview", "Projects", new { id = currentProjectId });
    }

    [HttpGet]
    public async Task<IActionResult> AssignMember(int bugReportId)
    {
        if (bugReportId < 1)
        {
            return BadRequest();
        }

        var currentProjectId = httpContextAccessor.HttpContext?.Session.GetInt32("currentProject");
        if (currentProjectId == null)
        {
            return NotFound();
        }

        var bugReport = await bugReportRepository.GetById(bugReportId);
        if (bugReport == null)
        {
            return NotFound();
        }

        var authorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User,
            currentProjectId, "ProjectAdministratorPolicy");
        if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
        {
            var currentProject = await projectRepository.GetById(currentProjectId.Value);
            ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.BugReportAssignMember(currentProject, bugReport);

            var assignedUsers = await bugReportRepository.GetAssignedUsersForBugReport(bugReportId);

            AssignMemberViewModel viewModel = new AssignMemberViewModel()
            {
                BugReportId = bugReportId,
                ProjectId = currentProjectId.Value,
                AssignedUsers = assignedUsers.ToList()
            };

            return View(viewModel);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> AssignMember(AssignMemberViewModel model)
    {
        if (model == null)
        {
            return BadRequest();
        }

        var authorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext?.User,
            model.ProjectId, "ProjectAdministratorPolicy");
        if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
        {
            var user = await userManager.FindByEmailAsync(model.MemberEmail);
            var assignedUserId = Int32.Parse(user.Id);

            await bugReportRepository.AddUserAssignedToBugReport(assignedUserId, model.BugReportId);

            // Create activity event
            var userId = int.Parse(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            var currentProjectId = httpContextAccessor.HttpContext?.Session.GetInt32("currentProject");
            if (currentProjectId == null)
            {
                return NotFound();
            }

            var activityEvent = new ActivityBugReportAssigned(DateTime.Now, currentProjectId.Value,
                ActivityMessage.BugReportAssignedToUser, userId, model.BugReportId, assignedUserId);
            await activityRepository.Add(activityEvent);

            return RedirectToAction("AssignMember", new { model.BugReportId });
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> RemoveAssignedMember(int projectId, int bugReportId, string memberEmail)
    {
        if (string.IsNullOrEmpty(memberEmail))
        {
            return BadRequest();
        }

        var authorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext?.User, projectId,
            "ProjectAdministratorPolicy");
        
        if (!(authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded))
            return RedirectToAction("Index", "Home");
            
        var user = await userManager.FindByEmailAsync(memberEmail);
        await bugReportRepository.DeleteUserAssignedToBugReport(int.Parse(user.Id), bugReportId);

        return RedirectToAction("AssignMember", new { bugReportId });
    }

    [HttpGet]
    public async Task<IActionResult> ManageLinks(int bugReportId)
    {
        if (bugReportId < 1)
        {
            return BadRequest();
        }

        var currentProjectId = httpContextAccessor.HttpContext?.Session.GetInt32("currentProject");
        if (currentProjectId == null)
        {
            return NotFound();
        }

        var bugReport = await bugReportRepository.GetById(bugReportId);
        if (bugReport == null)
        {
            return NotFound();
        }

        var authorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User,
            currentProjectId, "ProjectAdministratorPolicy");
        if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
        {
            var currentProject = await projectRepository.GetById(currentProjectId.Value);
            ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.BugReportManageLinks(currentProject, bugReport);

            var linkedReports = await bugReportRepository.GetLinkedReports(bugReportId);

            var model = new ManageLinksViewModel
            {
                ProjectId = currentProjectId.Value,
                BugReportId = bugReportId,
                LinkedReports = linkedReports.ToList()
            };

            return View(model);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> LinkReports(LinkReportsViewModel model)
    {
        if (model == null)
        {
            return BadRequest();
        }

        var currentProjectId = httpContextAccessor.HttpContext?.Session.GetInt32("currentProject");
        if (currentProjectId == null)
        {
            return NotFound();
        }

        var authorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User,
            model.ProjectId, "ProjectAdministratorPolicy");
        
        if (!(authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded))
            return RedirectToAction("Index", "Home");
        
        var linkToReport =
            await bugReportRepository.GetBugReportByLocalId(model.LinkToBugReportLocalId, model.ProjectId);
        await bugReportRepository.AddBugReportLink(model.BugReportId, linkToReport.BugReportId);

        // Create activity event
        var rawUserId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(rawUserId, out var userId)) return RedirectToAction("Index", "Home");
        
        var activityEvent = new ActivityBugReportLink(DateTime.Now, currentProjectId.Value,
            ActivityMessage.BugReportsLinked, userId, model.BugReportId, linkToReport.BugReportId);
        await activityRepository.Add(activityEvent);

        return RedirectToAction("ReportOverview", new { bugReportId = model.BugReportId });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteLink(int projectId, int bugReportId, int linkToBugReportId)
    {
        var authorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext?.User, projectId,
            "ProjectAdministratorPolicy");
        if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
        {
            await bugReportRepository.DeleteBugReportLink(bugReportId, linkToBugReportId);

            return RedirectToAction("ReportOverview", new { bugReportId = bugReportId });
        }

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> ReportOverview(int bugReportId)
    {
        if (bugReportId < 1)
        {
            return BadRequest();
        }
        
        var currentProjectId = httpContextAccessor.HttpContext?.Session.GetInt32("currentProject");
        if (currentProjectId == null)
        {
            return NotFound();
        }

        var authorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User,
            currentProjectId, "CanAccessProjectPolicy");
        
        if (!(authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded))
            return RedirectToAction("Index", "Home");
        
        httpContextAccessor.HttpContext.Session.SetInt32("currentBugReport", bugReportId);
        var bugReport = await bugReportRepository.GetById(bugReportId);

        var bugStates = await bugReportStatesRepository.GetAllById(bugReport.BugReportId);
        var bugStatesList = bugStates.OrderByDescending(o => o.Time).ToList();

        var assignedMembers = await bugReportRepository.GetAssignedUsersForBugReport(bugReportId);
        var assignedMembersList = assignedMembers.Select(x => new string(x.UserName)).ToList();

        var assignedMembersDisplay = assignedMembersList.Count > 0
            ? string.Join(", ", assignedMembersList)
            : "Unassigned";

        var comments = await commentRepository.GetAllById(bugReport.BugReportId);
        var activities = await activityRepository.GetBugReportActivities(bugReport.BugReportId);

        var bugViewModel = new OverviewBugReportViewModel
        {
            BugReport = bugReport,
            Comments = comments.ToList(),
            BugStates = bugStatesList,
            Activities = activities.ToList(),
            CurrentState = bugStatesList[0].StateType,
            AssignedMembersDisplay = assignedMembersDisplay
        };

        // Generate activity messages
        var activityMessageBuilder = new ActivityMessageBuilder(applicationLinkGenerator, userManager,
            projectRepository,
            bugReportRepository, milestoneRepository, bugReportStatesRepository);
        await activityMessageBuilder.GenerateMessages(bugViewModel.Activities);

        var rawUserId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(rawUserId, out var userId)) return Unauthorized(new { message = "Unauthorized" });
        
        if (await subscriptions.IsSubscribed(userId, bugViewModel.BugReport.BugReportId))
        {
            bugViewModel.DisableSubscribeButton = true;
        }

        var adminAuthorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User,
            currentProjectId, "ProjectAdministratorPolicy");
        if (adminAuthorizationResult.IsCompletedSuccessfully && adminAuthorizationResult.Result.Succeeded)
        {
            bugViewModel.DisableAssignMembersButton = false;
        }

        var currentProject = await projectRepository.GetById(currentProjectId.Value);

        ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.BugReportOverview(currentProject, bugReport.Title);

        return View(bugViewModel);
    }
}
