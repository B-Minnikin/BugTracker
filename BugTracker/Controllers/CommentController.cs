using BugTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
	public class CommentController : Controller
	{
		private readonly ILogger<CommentController> _logger;
		private readonly IProjectRepository projectRepository;

		public CommentController(ILogger<CommentController> logger,
									IProjectRepository projectRepository)
		{
			this._logger = logger;
			this.projectRepository = projectRepository;
		}

		[HttpGet]
		public ViewResult Create(int bugReportId)
		{
			BugReportComment newComment = new BugReportComment
			{
				BugReportId = bugReportId
			};

			return View(newComment);
		}

		[HttpPost]
		public IActionResult Create(BugReportComment model)
		{
			if (ModelState.IsValid)
			{
				BugReportComment newComment = new BugReportComment
				{
					Author = "User", // WIP user profiles
					Date = DateTime.Now,
					MainText = model.MainText,
					BugReportId = model.BugReportId
				};

				BugReportComment addedComment = projectRepository.CreateComment(newComment);
				return RedirectToAction("reportoverview", "bugreport", new { id = addedComment.BugReportId});
			}

			return View();
		}

		[HttpGet]
		public ViewResult Edit(int id)
		{
			BugReportComment bugReportComment = projectRepository.GetBugReportCommentById(id);

			return View(bugReportComment);
		}

		[HttpPost]
		public ViewResult Edit(BugReportComment model)
		{
			if (ModelState.IsValid)
			{
				BugReportComment comment = projectRepository.GetBugReportCommentById(model.BugReportCommentId);
				comment.MainText = model.MainText;
				// increment edit count
				// update edit time

				projectRepository.UpdateButReportComment(comment);
				return RedirectToAction("ReportOverview", "BugReport", new { });
			}

			return View();
		}

		public ViewResult Delete(int id)
		{
			return View();
		}

	}
}
