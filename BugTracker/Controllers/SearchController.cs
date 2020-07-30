using BugTracker.Models;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
	public class SearchController : Controller
	{
		private readonly ILogger<SearchController> logger;
		private readonly IProjectRepository projectRepository;
		private readonly IHttpContextAccessor httpContextAccessor;

		public SearchController(ILogger<SearchController> logger,
									  IProjectRepository projectRepository,
									  IHttpContextAccessor httpContextAccessor)
		{
			this.logger = logger;
			this.projectRepository = projectRepository;
			this.httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		public ViewResult Search()
		{
			SearchResultsViewModel searchResultsViewModel = new SearchResultsViewModel
			{
				SearchResults = new List<BugReport>()
			};

			return View(searchResultsViewModel);
		}

		[HttpPost]
		public ViewResult Result(SearchResultsViewModel searchModel)
		{
			int currentProjectId = (int)HttpContext.Session.GetInt32("currentProject");
			var bugReports = projectRepository.GetAllBugReports(currentProjectId);

			if (!String.IsNullOrEmpty(searchModel.SearchExpression.SearchText))
			{
				searchModel.SearchResults = bugReports.Where(rep => rep.Title.Contains(searchModel.SearchExpression.SearchText)).ToList();
			}

			return View(searchModel);
		}
	}
}
