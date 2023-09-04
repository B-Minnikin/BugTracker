using BugTracker.Helpers;
using BugTracker.Models;
using BugTracker.Repository.Interfaces;
using BugTracker.Services;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
		private readonly IBugReportRepository bugReportRepository;
		private readonly ISearchRepository searchRepository;
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly IAuthorizationService authorizationService;
		private readonly ILinkGenerator linkGenerator;

		public SearchController(ILogger<SearchController> logger,
									  IProjectRepository projectRepository,
									  IBugReportRepository bugReportRepository,
									  ISearchRepository searchRepository,
									  IHttpContextAccessor httpContextAccessor,
									  IAuthorizationService authorizationService,
									  ILinkGenerator linkGenerator)
		{
			this.logger = logger;
			this.projectRepository = projectRepository;
			this.bugReportRepository = bugReportRepository;
			this.searchRepository = searchRepository;
			this.httpContextAccessor = httpContextAccessor;
			this.authorizationService = authorizationService;
			this.linkGenerator = linkGenerator;
		}

		[HttpPost]
		public async Task<IActionResult> Result(SearchResultsViewModel searchModel)
		{
			if(searchModel is null)
			{
				throw new ArgumentNullException(nameof(searchModel));
			}

			var currentProjectId = httpContextAccessor.HttpContext?.Session.GetInt32("currentProject");
			if (!currentProjectId.HasValue) return BadRequest();
			
			logger.LogInformation($"currentProjectId = {currentProjectId.Value}");
			if(currentProjectId <= 0) {
				logger.LogWarning($"currentProjectId ({currentProjectId}) not set to a valid value: Redirecting to Home controller.");
				return RedirectToAction("Index", "Home");
			}

			var authorizationResult = authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User, currentProjectId, "CanAccessProjectPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				if (ModelState.IsValid)
				{
					var project = await projectRepository.GetById(currentProjectId.Value);
					var bugReports = await bugReportRepository.GetAllById(project.ProjectId);

					// set default start date to the project's creation date - if user has not entered more recent date
					if (searchModel.SearchExpression.DateRangeBegin < project.CreationTime)
						searchModel.SearchExpression.DateRangeBegin = project.CreationTime;

					if (!string.IsNullOrEmpty(searchModel.SearchExpression.SearchText))
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
		public async Task<IActionResult> GetProjectMembers(string query, int projectId)
		{
			if (query is null)
			{
				throw new ArgumentNullException(nameof(query));
			}

			IEnumerable<UserTypeaheadSearchResult> userSearchResults = new List<UserTypeaheadSearchResult>();

			var user = httpContextAccessor.HttpContext?.User;
			if (user is null) return BadRequest();

			var authorizationResult = authorizationService.AuthorizeAsync(user, projectId, "CanAccessProjectPolicy");
			if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
			{
				if (!string.IsNullOrEmpty(query) && projectId > 0)
					userSearchResults = await searchRepository.GetMatchingProjectMembersBySearchQuery(query.ToUpper(), projectId);
			}

			return Json(userSearchResults.ToList());
		}

		[HttpGet]
		public async Task<IActionResult> GetBugReports(string query, int projectId)
		{
			IEnumerable<BugReportTypeaheadSearchResult> bugReportSearchResults = new List<BugReportTypeaheadSearchResult>();

			var authorizationResult = await authorizationService.AuthorizeAsync(httpContextAccessor.HttpContext.User, projectId, "CanAccessProjectPolicy");
			if ((!string.IsNullOrEmpty(query) && projectId > 0) & (authorizationResult.Succeeded))
			{
				query = query.TrimStart('#'); // remove leading # for local report ID queries
				
				var intParseSuccess = int.TryParse(query, out var localBugReportId);

				bugReportSearchResults = intParseSuccess
					? await searchRepository.GetMatchingBugReportsByLocalIdSearchQuery(localBugReportId, projectId)
					: await searchRepository.GetMatchingBugReportsByTitleSearchQuery(query.ToUpper(), projectId);
			}

			// Generate an URL for each bug report
			foreach(var result in bugReportSearchResults.ToList())
			{
				result.Url = GetUrl(result.BugReportId);
			}

			return Json(bugReportSearchResults);
		}

		private string GetUrl(int bugReportId)
		{
			var result = linkGenerator.GetPathByAction("ReportOverview", "BugReport", new { bugReportId });
			return result;
		}
	}
}
