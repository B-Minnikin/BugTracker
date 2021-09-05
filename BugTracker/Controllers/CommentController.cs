using BugTracker.Models;
using BugTracker.Models.Database;
using BugTracker.Repository;
using BugTracker.Repository.Interfaces;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartBreadcrumbs.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
	public class CommentController : Controller
	{
		private readonly ILogger<CommentController> _logger;
		private readonly IProjectRepository projectRepository;
		private readonly IBugReportRepository bugReportRepository;
		private readonly IUserSubscriptionsRepository userSubscriptionsRepository;
		private readonly IActivityRepository activityRepository;
		private readonly IAuthorizationService authorizationService;
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly ISubscriptions subscriptions;

		public CommentController(ILogger<CommentController> logger,
									IProjectRepository projectRepository,
									IBugReportRepository bugReportRepository,
									IUserSubscriptionsRepository userSubscriptionsRepository,
									IActivityRepository activityRepository,
									IAuthorizationService authorizationService,
									IHttpContextAccessor httpContextAccessor,
									ISubscriptions subscriptions)
		{
			this._logger = logger;
			this.projectRepository = projectRepository;
			this.bugReportRepository = bugReportRepository;
			this.userSubscriptionsRepository = userSubscriptionsRepository;
			this.activityRepository = activityRepository;
			this.authorizationService = authorizationService;
			this.httpContextAccessor = httpContextAccessor;
			this.subscriptions = subscriptions;
		}

		[HttpGet]
		public IActionResult Create(int bugReportId)
		{
			var currentProjectId = HttpContext.Session.GetInt32("currentProject");

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProjectId, "CanAccessProjectPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				CreateCommentViewModel createCommentViewModel = new CreateCommentViewModel{};
				createCommentViewModel.Comment.BugReportId = bugReportId;

				var currentProject = projectRepository.GetProjectById(currentProjectId ?? 0);
				var currentBugReportId = HttpContext.Session.GetInt32("currentBugReport");
				var currentBugReport = bugReportRepository.GetById(currentBugReportId ?? 0);

				// --------------------- CONFIGURE BREADCRUMB NODES ----------------------------
				var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
				var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", currentProject.Name)
				{
					RouteValues = new { id = currentProjectId },
					Parent = projectsNode
				};
				var reportNode = new MvcBreadcrumbNode("ReportOverview", "BugReport", currentBugReport.Title)
				{
					RouteValues = new { id = currentBugReportId },
					Parent = overviewNode
				};
				var commentNode = new MvcBreadcrumbNode("Create", "Comment", "Comment")
				{
					Parent = reportNode
				};
				ViewData["BreadcrumbNode"] = commentNode;
				// --------------------------------------------------------------------------------------------

				return View(createCommentViewModel);
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public IActionResult Create(CreateCommentViewModel model)
		{
			if (ModelState.IsValid)
			{
				BugReportComment newComment = new BugReportComment
				{
					Author = HttpContext.User.Identity.Name,
					Date = DateTime.Now,
					MainText = model.Comment.MainText,
					BugReportId = model.Comment.BugReportId
				};

				BugReportComment addedComment = projectRepository.CreateComment(newComment);
				int userId = Int32.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

				// Create activity event
				var currentProjectId = HttpContext.Session.GetInt32("currentProject");
				var commentActivity = new ActivityComment(DateTime.Now, currentProjectId.Value, ActivityMessage.CommentPosted, userId, addedComment.BugReportId, addedComment.BugReportCommentId);
				activityRepository.Add(commentActivity);

				if (model.Subscribe && !subscriptions.IsSubscribed(userId, addedComment.BugReportId))
				{
					userSubscriptionsRepository.AddSubscription(userId, addedComment.BugReportId);
				}

				string bugReportUrl = Url.Action("ReportOverview", "BugReport", new { id = addedComment.BugReportId}, Request.Scheme);
				subscriptions.NotifyBugReportNewComment(addedComment, bugReportUrl);

				return RedirectToAction("ReportOverview", "BugReport", new { id = addedComment.BugReportId});
			}

			return View();
		}

		[HttpGet]
		public ViewResult Edit(int id)
		{
			BugReportComment bugReportComment = projectRepository.GetBugReportCommentById(id);

			var currentProjectId = HttpContext.Session.GetInt32("currentProject");
			var currentProject = projectRepository.GetProjectById(currentProjectId ?? 0);
			var currentBugReportId = HttpContext.Session.GetInt32("currentBugReport");
			var currentBugReport = bugReportRepository.GetById(currentBugReportId ?? 0);

			// --------------------- CONFIGURE BREADCRUMB NODES ----------------------------
			var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
			var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", currentProject.Name)
			{
				RouteValues = new { id = currentProjectId },
				Parent = projectsNode
			};
			var reportNode = new MvcBreadcrumbNode("ReportOverview", "BugReport", currentBugReport.Title)
			{
				RouteValues = new { id = currentBugReportId },
				Parent = overviewNode
			};
			var commentNode = new MvcBreadcrumbNode("Edit", "Comment", "Edit Comment")
			{
				Parent = reportNode
			};
			ViewData["BreadcrumbNode"] = commentNode;
			// --------------------------------------------------------------------------------------------

			return View(bugReportComment);

		}

		[HttpPost]
		public IActionResult Edit(BugReportComment model)
		{
			if (ModelState.IsValid)
			{
				BugReportComment comment = projectRepository.GetBugReportCommentById(model.BugReportCommentId);
				var currentProjectId = HttpContext.Session.GetInt32("currentProject");

				var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, new { ProjectId = currentProjectId, Author = comment.Author}, "CanModifyCommentPolicy");
				if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
				{
					comment.MainText = model.MainText;
					// increment edit count
					// update edit time

					projectRepository.UpdateBugReportComment(comment);

					// Create activity event
					int userId = Int32.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
					var commentActivity = new ActivityComment(DateTime.Now, currentProjectId.Value, ActivityMessage.CommentEdited, userId, comment.BugReportId, comment.BugReportCommentId);
					activityRepository.Add(commentActivity);
				}
				return RedirectToAction("ReportOverview", "BugReport", new { id = comment.BugReportId});
			}

			return View();
		}

		public IActionResult Delete(int id)
		{
			var currentProjectId = HttpContext.Session.GetInt32("currentProject");
			int parentBugReportId = projectRepository.GetCommentParentId(id);
			string commentAuthor = projectRepository.GetBugReportCommentById(id).Author;

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, new { ProjectId = currentProjectId, Author = commentAuthor }, "CanModifyCommentPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				projectRepository.DeleteComment(id);
			}

			return RedirectToAction("ReportOverview", "BugReport", new { id = parentBugReportId });
		}
	}
}
