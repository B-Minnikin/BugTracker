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
		public ViewResult Create()
		{
			return View();
		}

		[HttpPost]
		public ViewResult Create(BugReportComment model)
		{
			return View();
		}

		[HttpGet]
		public ViewResult Edit(int id)
		{
			return View();
		}

		[HttpPost]
		public ViewResult Edit(BugReportComment model)
		{
			return View();
		}

		public ViewResult Delete(int id)
		{
			return View();
		}

	}
}
