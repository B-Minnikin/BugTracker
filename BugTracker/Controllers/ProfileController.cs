using BugTracker.Helpers;
using BugTracker.Models.Authorization;
using BugTracker.Repository.Interfaces;
using BugTracker.Services;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using SmartBreadcrumbs.Attributes;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BugTracker.Models.Subscription;

namespace BugTracker.Controllers;

public class ProfileController : Controller
{
	private readonly ILogger<ProfileController> logger;
	private readonly IAuthorizationService authorizationService;
	private readonly ISubscriptions subscriptions;
	private readonly IHttpContextAccessor httpContextAccessor;
	private readonly LinkGenerator linkGenerator;
	private readonly ApplicationUserManager userManager;
	private readonly IProjectRepository projectRepository;
	private readonly IMilestoneRepository milestoneRepository;
	private readonly IBugReportRepository bugReportRepository;
	private readonly IBugReportStatesRepository bugReportStatesRepository;
	private readonly IUserSubscriptionsRepository userSubscriptionsRepository;
	private readonly IActivityRepository activityRepository;

	public ProfileController(ILogger<ProfileController> logger,
								IProjectRepository projectRepository,
								IMilestoneRepository milestoneRepository,
								IBugReportRepository bugReportRepository,
								IBugReportStatesRepository bugReportStatesRepository,
								IUserSubscriptionsRepository userSubscriptionsRepository,
								IActivityRepository activityRepository,
								IAuthorizationService authorizationService,
								ISubscriptions subscriptions,
								IHttpContextAccessor httpContextAccessor,
								LinkGenerator linkGenerator,
								ApplicationUserManager userManager)
	{
		this.logger = logger;
		this.authorizationService = authorizationService;
		this.subscriptions = subscriptions;
		this.httpContextAccessor = httpContextAccessor;
		this.linkGenerator = linkGenerator;
		this.userManager = userManager;
		this.projectRepository = projectRepository;
		this.milestoneRepository = milestoneRepository;
		this.bugReportRepository = bugReportRepository;
		this.bugReportStatesRepository = bugReportStatesRepository;
		this.userSubscriptionsRepository = userSubscriptionsRepository;
		this.activityRepository = activityRepository;
	}

	[Breadcrumb("My Profile", FromController = typeof(HomeController))]
	public async Task<ViewResult> View(string userId)
	{
		//var userManager = new ApplicationUserManager();
		var activities = await activityRepository.GetUserActivities(userId);

		var viewModel = new EditProfileViewModel
		{
			User = await userManager.FindByIdAsync(userId),
			Activities = activities.ToList()
		};

		// generate activity messages
		var applicationLinkGenerator = new ApplicationLinkGenerator(httpContextAccessor, linkGenerator);
		var activityMessageBuilder = new ActivityMessageBuilder(applicationLinkGenerator, userManager, projectRepository, 
			bugReportRepository, milestoneRepository, bugReportStatesRepository);
		await activityMessageBuilder.GenerateMessages(viewModel.Activities);

		return View(viewModel);
	}

	[Authorize]
	public async Task<ViewResult> Subscriptions(string userId)
	{
		var bugReports = await userSubscriptionsRepository.GetSubscribedReports(userId);

		var subscriptionsViewModel = new SubscriptionsViewModel
		{
			BugReports = bugReports.ToList()
		};

		ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.ProfileSubscriptions(userId);

		return View(subscriptionsViewModel);
	}

	public IActionResult DeleteSubscription(int bugReportId)
	{
		var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		subscriptions.DeleteSubscription(userId, bugReportId);
		logger.LogWarning("Subscription removed");

		return RedirectToAction("Subscriptions", new { id = userId});
	}

	[HttpGet]
	[Authorize]
	public async Task<IActionResult> Edit(int id)
	{
		var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, id, "CanModifyProfilePolicy");
		if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
		{
			var user = await userManager.FindByIdAsync(id.ToString());
			var profileViewModel = new EditProfileViewModel
			{
				User = user
			};

			ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.ProfileEdit(id);

			return View(profileViewModel);
		}

		return RedirectToAction("Index", "Home");
	}

	[HttpPost]
	[Authorize]
	public async Task<IActionResult> Edit(EditProfileViewModel model)
	{
		var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, Int32.Parse(model.User.Id), "CanModifyProfilePolicy");
		if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
		{
			if (ModelState.IsValid)
			{
				var user = await userManager.FindByIdAsync(model.User.Id);
				user.PhoneNumber = model.User.PhoneNumber;

				var result = await userManager.UpdateAsync(user);
				if (!result.Succeeded)
				{
					logger.LogError($"Failed to update user profile [{model.User.Id}]");
					return (IActionResult)View("Error");
				}
			}
		}

		return RedirectToAction("Index", "Home");
	}
}
