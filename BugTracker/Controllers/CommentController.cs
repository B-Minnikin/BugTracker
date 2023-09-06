using BugTracker.Helpers;
using BugTracker.Models;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BugTracker.Database.Repository.Interfaces;
using BugTracker.Models.Subscription;

namespace BugTracker.Controllers
{
	public class CommentController : Controller
	{
		private readonly IProjectRepository projectRepository;
		private readonly IBugReportRepository bugReportRepository;
		private readonly IUserSubscriptionsRepository userSubscriptionsRepository;
		private readonly IActivityRepository activityRepository;
		private readonly ICommentRepository commentRepository;
		private readonly IAuthorizationService authorizationService;
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly ISubscriptions subscriptions;

		public CommentController(IProjectRepository projectRepository,
									IBugReportRepository bugReportRepository,
									IUserSubscriptionsRepository userSubscriptionsRepository,
									IActivityRepository activityRepository,
									ICommentRepository commentRepository,
									IAuthorizationService authorizationService,
									IHttpContextAccessor httpContextAccessor,
									ISubscriptions subscriptions)
		{
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
				var createCommentViewModel = new CreateCommentViewModel
				{
					Comment =
					{
						BugReportId = bugReportId
					}
				};

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
				var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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
				if (!currentProjectId.HasValue) return BadRequest();
				
				var commentActivity = new ActivityComment(DateTime.Now, currentProjectId.Value, ActivityMessage.CommentPosted, userId, addedComment.BugReportId, addedComment.CommentId);
				await activityRepository.Add(commentActivity);

				if (model.Subscribe && !await subscriptions.IsSubscribed(userId, addedComment.BugReportId))
				{
					await userSubscriptionsRepository.AddSubscription(userId, addedComment.BugReportId);
				}

				string bugReportUrl = Url.Action("ReportOverview", "BugReport", new { bugReportId = addedComment.BugReportId}, Request.Scheme);
				await subscriptions .NotifyBugReportNewComment(addedComment, bugReportUrl);

				return RedirectToAction("ReportOverview", "BugReport", new { bugReportId = addedComment.BugReportId});
			}

			return View();
		}

		[HttpGet]
		public async Task<ViewResult> Edit(int id)
		{
			var comment = await commentRepository.GetById(id);

			var currentProjectId = HttpContext.Session.GetInt32("currentProject");
			var currentProject = await projectRepository.GetById(currentProjectId ?? 0);
			var currentBugReportId = HttpContext.Session.GetInt32("currentBugReport");
			var currentBugReport = await bugReportRepository .GetById(currentBugReportId ?? 0);

			ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.CommentEdit(currentProject, currentBugReport);

			return View(comment);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(Comment viewModel)
		{
			if (ModelState.IsValid)
			{
				var comment = await commentRepository .GetById(viewModel.CommentId);
				var currentProjectId = HttpContext.Session.GetInt32("currentProject");
				if (!currentProjectId.HasValue) return BadRequest();

				var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, new { ProjectId = currentProjectId, Author = comment.AuthorId}, "CanModifyCommentPolicy");
				if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
				{
					comment.MainText = viewModel.MainText;
					// increment edit count
					// update edit time

					await commentRepository .Update(comment);

					// Create activity event
					var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
					var commentActivity = new ActivityComment(DateTime.Now, currentProjectId.Value, ActivityMessage.CommentEdited, userId, comment.BugReportId, comment.CommentId);
					await activityRepository.Add(commentActivity);
				}
				return RedirectToAction("ReportOverview", "BugReport", new { bugReportId = comment.BugReportId});
			}

			return View();
		}

		public async Task<IActionResult> Delete(int id)
		{
			var currentProjectId = HttpContext.Session.GetInt32("currentProject");
			var parentBugReportId = await commentRepository.GetCommentParentId(id);
			var commentAuthor = await commentRepository.GetById(id);
			var commentAuthorId = commentAuthor.AuthorId;

			var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, new { ProjectId = currentProjectId, Author = commentAuthorId }, "CanModifyCommentPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				await commentRepository.Delete(id);
			}

			return RedirectToAction("ReportOverview", "BugReport", new { bugReportId = parentBugReportId });
		}
	}
}
