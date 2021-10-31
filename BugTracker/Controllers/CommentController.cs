using BugTracker.Helpers;
using BugTracker.Models;
using BugTracker.Models.Database;
using BugTracker.Repository;
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

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProjectId, "CanAccessProjectPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				CreateCommentViewModel createCommentViewModel = new CreateCommentViewModel{};
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
			if (ModelState.IsValid)
			{
				int userId = Int32.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

				Comment newComment = new Comment
				{
					AuthorId = userId,
					Date = DateTime.Now,
					MainText = model.Comment.MainText,
					BugReportId = model.Comment.BugReportId
				};

				Comment addedComment = await commentRepository.Add(newComment);

				// Create activity event
				var currentProjectId = HttpContext.Session.GetInt32("currentProject");
				var commentActivity = new ActivityComment(DateTime.Now, currentProjectId.Value, ActivityMessage.CommentPosted, userId, addedComment.BugReportId, addedComment.CommentId);
				await activityRepository.Add(commentActivity);

				if (model.Subscribe && !subscriptions.IsSubscribed(userId, addedComment.BugReportId))
				{
					await userSubscriptionsRepository.AddSubscription(userId, addedComment.BugReportId);
				}

				string bugReportUrl = Url.Action("ReportOverview", "BugReport", new { id = addedComment.BugReportId}, Request.Scheme);
				await subscriptions .NotifyBugReportNewComment(addedComment, bugReportUrl);

				return RedirectToAction("ReportOverview", "BugReport", new { id = addedComment.BugReportId});
			}

			return View();
		}

		[HttpGet]
		public async Task<ViewResult> Edit(int id)
		{
			Comment comment = await commentRepository .GetById(id);

			var currentProjectId = HttpContext.Session.GetInt32("currentProject");
			var currentProject = await projectRepository.GetById(currentProjectId ?? 0);
			var currentBugReportId = HttpContext.Session.GetInt32("currentBugReport");
			var currentBugReport = await bugReportRepository .GetById(currentBugReportId ?? 0);

			ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.CommentEdit(currentProject, currentBugReport);

			return View(comment);

		}

		[HttpPost]
		public async Task<IActionResult> Edit(Comment model)
		{
			if (ModelState.IsValid)
			{
				Comment comment = await commentRepository .GetById(model.CommentId);
				var currentProjectId = HttpContext.Session.GetInt32("currentProject");

				var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, new { ProjectId = currentProjectId, Author = comment.AuthorId}, "CanModifyCommentPolicy");
				if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
				{
					comment.MainText = model.MainText;
					// increment edit count
					// update edit time

					await commentRepository .Update(comment);

					// Create activity event
					int userId = Int32.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
					var commentActivity = new ActivityComment(DateTime.Now, currentProjectId.Value, ActivityMessage.CommentEdited, userId, comment.BugReportId, comment.CommentId);
					await activityRepository.Add(commentActivity);
				}
				return RedirectToAction("ReportOverview", "BugReport", new { id = comment.BugReportId});
			}

			return View();
		}

		public async Task<IActionResult> Delete(int id)
		{
			var currentProjectId = HttpContext.Session.GetInt32("currentProject");
			int parentBugReportId = await commentRepository.GetCommentParentId(id);
			var commentAuthor = await commentRepository .GetById(id);
			int commentAuthorId = commentAuthor.AuthorId;

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, new { ProjectId = currentProjectId, Author = commentAuthorId }, "CanModifyCommentPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				await commentRepository .Delete(id);
			}

			return RedirectToAction("ReportOverview", "BugReport", new { id = parentBugReportId });
		}
	}
}
