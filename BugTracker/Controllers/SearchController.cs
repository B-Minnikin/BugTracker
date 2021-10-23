using BugTracker.Helpers;
using BugTracker.Models;
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
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
	public class SearchController : Controller
	{
		private readonly ILogger<SearchController> logger;
		private readonly IProjectRepository projectRepository;
		private readonly IBugReportRepository bugReportRepository;
		private readonly ISearchRepository searchRepository;
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly IAuthorizationService authorizationService;

		public SearchController(ILogger<SearchController> logger,
									  IProjectRepository projectRepository,
									  IBugReportRepository bugReportRepository,
									  ISearchRepository searchRepository,
									  IHttpContextAccessor httpContextAccessor,
									  IAuthorizationService authorizationService)
		{
			this.logger = logger;
			this.projectRepository = projectRepository;
			this.bugReportRepository = bugReportRepository;
			this.searchRepository = searchRepository;
			this.httpContextAccessor = httpContextAccessor;
			this.authorizationService = authorizationService;
		}

		[HttpPost]
		public IActionResult Result(SearchResultsViewModel searchModel)
		{
			int currentProjectId = httpContextAccessor.HttpContext.Session.GetInt32("currentProject") ?? 0;
			logger.LogInformation("currentProjectId = " + currentProjectId);
			if(currentProjectId <= 0) {
				logger.LogWarning($"currentProjectId ({currentProjectId}) not set to a valid value: Redirecting to Home controller.");
				return RedirectToAction("Index", "Home");
			}

			var authorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User, currentProjectId, "CanAccessProjectPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				if (ModelState.IsValid)
				{
					var project = projectRepository.GetById(currentProjectId);
					var bugReports = bugReportRepository.GetAllById(project.ProjectId);

					// set default start date to the project's creation date - if user has not entered more recent date
					if (searchModel.SearchExpression.DateRangeBegin < project.CreationTime)
						searchModel.SearchExpression.DateRangeBegin = project.CreationTime;

					if (!String.IsNullOrEmpty(searchModel.SearchExpression.SearchText))
					{
						searchModel.SearchResults = bugReports.Where(rep => rep.Title.ToUpper().Contains(searchModel.SearchExpression.SearchText.ToUpper())
							&& rep.CreationTime >= searchModel.SearchExpression.DateRangeBegin && rep.CreationTime <= searchModel.SearchExpression.DateRangeEnd).ToList();
					}

					ViewData["BreadcrumbNode"] = BreadcrumbNodeHelper.SearchResult(project.ProjectId, project.Name);
				}
					
				return View(searchModel);
			}

			logger.LogError("Authorization failed for user: {0} inside SearchController.Results", httpContextAccessor.HttpContext.User);
			return View();
		}

		[HttpGet]
		public IActionResult GetProjectMembers(string query, int projectId)
		{
			List<UserTypeaheadSearchResult> userSearchResults = new List<UserTypeaheadSearchResult>();

			if(!string.IsNullOrEmpty(query) && projectId > 0)
				userSearchResults = searchRepository.GetMatchingProjectMembersBySearchQuery(query.ToUpper(), projectId).ToList();

			return Json(userSearchResults);
		}

		[HttpGet]
		public IActionResult GetBugReports(string query, int projectId)
		{
			List<BugReportTypeaheadSearchResult> bugReportSearchResults = new List<BugReportTypeaheadSearchResult>();

			if (!string.IsNullOrEmpty(query) && projectId > 0)
			{
				query = query.TrimStart('#'); // remove leading # for local report ID queries
				int localBugReportId;
				bool intParseSuccess = Int32.TryParse(query, out localBugReportId);

				if(intParseSuccess)
					bugReportSearchResults = searchRepository.GetMatchingBugReportsByLocalIdSearchQuery(localBugReportId, projectId).ToList();
				else
					bugReportSearchResults = searchRepository.GetMatchingBugReportsByTitleSearchQuery(query.ToUpper(), projectId).ToList();
			}

			// Generate an URL for each bug report
			foreach(var result in bugReportSearchResults)
			{
				result.Url = Url.Action("ReportOverview", "BugReport", new { id = result.BugReportId }, Request.Scheme);
			}

			return Json(bugReportSearchResults);
		}
	}
}
