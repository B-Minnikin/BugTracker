using BugTracker.Helpers;
using BugTracker.Models;
using BugTracker.Models.Subscription;
using BugTracker.Repository.Interfaces;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
		private readonly ICommentRepository commentRepository;
		private readonly IAuthorizationService authorizationService;
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly ISubscriptions subscriptions;

		public CommentController(ILogger<CommentController> logger,
									IProjectRepository projectRepository,
									IBugReportRepository bugReportRepository,
									IUserSubscriptionsRepository userSubscriptionsRepository,
									IActivityRepository activityRepository,
									ICommentRepository commentRepository,
									IAuthorizationService authorizationService,
									IHttpContextAccessor httpContextAccessor,
									ISubscriptions subscriptions)
		{
			this._logger = logger;
			this.projectRepository = projectRepository;
			this.bugReportRepository = bugReportRepository;
			this.userSubscriptionsRepository = userSubscriptionsRepository;
			this.activityRepository = activityRepository;
			this.commentRepository = commentRepository;
			this.authorizationService = authorizationService;
			this.httpContextAccessor = httpContextAccessor;
			this.subscriptions = subscriptions;
		}

		[HttpGet]
		public async Task<IActionResult> Create(int bugReportId)
		{
			var currentProjectId = HttpContext.Session.GetInt32("currentProject");
			if (currentProjectId == null) return NotFound();

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProjectId, "CanAccessProjectPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				var createCommentViewModel = new CreateCommentViewModel{};
				createCommentViewModel.Comment.BugReportId = bugReportId;

				var currentProject = await projectRepository.GetById(currentProjectId ?? 0);
				var currentBugReportId = HttpContext.Session.GetInt32("currentBugReport");
				var currentBugReport = await bugReportRepository .GetById(currentBugReportId ?? 0);

				ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.CommentCreate(currentProject, currentBugReport);

				return View(createCommentViewModel);
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public async Task<IActionResult> Create(CreateCommentViewModel model)
		{
			if (!ModelState.IsValid) return View();
			
			var rawUserId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (!int.TryParse(rawUserId, out var userId)) return View();

			var newComment = new Comment
			{
				AuthorId = userId,
				Date = DateTime.Now,
				MainText = model.Comment.MainText,
				BugReportId = model.Comment.BugReportId
			};

			var addedComment = await commentRepository.Add(newComment);

			// Create activity event
			var currentProjectId = HttpContext.Session.GetInt32("currentProject");
			if (currentProjectId == null) return NotFound();
			
			var commentActivity = new ActivityComment(DateTime.Now, currentProjectId.Value, ActivityMessage.CommentPosted, userId, addedComment.BugReportId, addedComment.CommentId);
			await activityRepository.Add(commentActivity);

			if (model.Subscribe && !await subscriptions.IsSubscribed(userId, addedComment.BugReportId))
			{
				await userSubscriptionsRepository.AddSubscription(userId, addedComment.BugReportId);
			}

			var bugReportUrl = Url.Action("ReportOverview", "BugReport", new { bugReportId = addedComment.BugReportId}, Request.Scheme);
			await subscriptions .NotifyBugReportNewComment(addedComment, bugReportUrl);

			return RedirectToAction("ReportOverview", "BugReport", new { bugReportId = addedComment.BugReportId});
		}

		[HttpGet]
		public async Task<IActionResult> Edit(int commentId)
		{
			var comment = await commentRepository.GetById(commentId);
			if (comment == null) return NotFound();
			
			// TODO - we don't need the full project and bug report just for the breadcrumbs

			var currentProjectId = HttpContext.Session.GetInt32("currentProject");
			if (currentProjectId == null) return NotFound();
			
			var currentProject = await projectRepository.GetById(currentProjectId.Value);
			if (currentProject == null) return NotFound();
			
			var currentBugReportId = HttpContext.Session.GetInt32("currentBugReport");
			if (currentBugReportId == null) return NotFound();
			
			var currentBugReport = await bugReportRepository .GetById(currentBugReportId.Value);

			ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.CommentEdit(currentProject, currentBugReport);

			return View(comment);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(Comment model)
		{
			if (!ModelState.IsValid) return View();
			
			var comment = await commentRepository .GetById(model.CommentId);
			var currentProjectId = HttpContext.Session.GetInt32("currentProject");
			if (currentProjectId == null) return NotFound();

			var authorizationResult = await authorizationService.AuthorizeAsync(HttpContext.User, new { ProjectId = currentProjectId, Author = comment.AuthorId}, "CanModifyCommentPolicy");
			if (authorizationResult.Succeeded)
			{
				comment.MainText = model.MainText;
				// Increment edit count
				// Update edit time

				await commentRepository .Update(comment);

				// Create activity event
				var rawUserId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (!int.TryParse(rawUserId, out var userId)) return View();
				var commentActivity = new ActivityComment(DateTime.Now, currentProjectId.Value, ActivityMessage.CommentEdited, userId, comment.BugReportId, comment.CommentId);
				await activityRepository.Add(commentActivity);
			}
			
			return RedirectToAction("ReportOverview", "BugReport", new { bugReportId = comment.BugReportId});
		}

		public async Task<IActionResult> Delete(int commentId)
		{
			var currentProjectId = HttpContext.Session.GetInt32("currentProject");
			var parentBugReportId = await commentRepository.GetCommentParentId(commentId);
			var commentAuthor = await commentRepository .GetById(commentId);
			var commentAuthorId = commentAuthor.AuthorId;

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, new { ProjectId = currentProjectId, Author = commentAuthorId }, "CanModifyCommentPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				await commentRepository .Delete(commentId);
			}

			return RedirectToAction("ReportOverview", "BugReport", new { bugReportId = parentBugReportId });
		}
	}
}
