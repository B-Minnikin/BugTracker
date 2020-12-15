using BugTracker.Models;
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
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly IAuthorizationService authorizationService;

		public SearchController(ILogger<SearchController> logger,
									  IProjectRepository projectRepository,
									  IHttpContextAccessor httpContextAccessor,
									  IAuthorizationService authorizationService)
		{
			this.logger = logger;
			this.projectRepository = projectRepository;
			this.httpContextAccessor = httpContextAccessor;
			this.authorizationService = authorizationService;
		}

		[HttpPost]
		public ViewResult Result(SearchResultsViewModel searchModel)
		{
			int? currentProjectId = HttpContext.Session.GetInt32("currentProject");
			if(currentProjectId != null)
			{
				logger.LogInformation("currentProjectId = " + currentProjectId);

				var authorizationResult = authorizationService.AuthorizeAsync(HttpContext.User, currentProjectId, "CanAccessProjectPolicy");
				if (authorizationResult.IsCompletedSuccessfully && authorizationResult.Result.Succeeded)
				{
					var bugReports = projectRepository.GetAllBugReports(currentProjectId.Value);

					// set default start date to the project's creation date - if user has not entered more recent date
					DateTime projectCreationTime = GetProjectCreationTime((int)currentProjectId);
					if (searchModel.SearchExpression.DateRangeBegin < projectCreationTime)
						searchModel.SearchExpression.DateRangeBegin = projectCreationTime;

					if (!String.IsNullOrEmpty(searchModel.SearchExpression.SearchText))
					{
						searchModel.SearchResults = bugReports.Where(rep => rep.Title.ToUpper().Contains(searchModel.SearchExpression.SearchText.ToUpper())
							&& rep.CreationTime >= searchModel.SearchExpression.DateRangeBegin && rep.CreationTime <= searchModel.SearchExpression.DateRangeEnd).ToList();
					}

					// --------------------- CONFIGURE BREADCRUMB NODES ----------------------------
					string currentProjectName = projectRepository.GetProjectById(currentProjectId.Value).Name;
					var projectsNode = new MvcBreadcrumbNode("Projects", "Projects", "Projects");
					var overviewNode = new MvcBreadcrumbNode("Overview", "Projects", currentProjectName)
					{
						RouteValues = new { id = currentProjectId.Value },
						Parent = projectsNode
					};
					var searchNode = new MvcBreadcrumbNode("Result", "SearchController", "Search")
					{
						Parent = overviewNode
					};
					ViewData["BreadcrumbNode"] = searchNode;
					// --------------------------------------------------------------------------------------------

					return View(searchModel);
				}
			}

			return View();
		}

		[HttpGet]
		public IActionResult GetProjectMembers(string query, int projectId)
		{
			List<UserTypeaheadSearchResult> userSearchResults = new List<UserTypeaheadSearchResult>();

			if(!string.IsNullOrEmpty(query) && projectId > 0)
				userSearchResults = projectRepository.GetMatchingProjectMembersBySearchQuery(query.ToUpper(), projectId).ToList();

			return Json(userSearchResults);
		}

		private DateTime GetProjectCreationTime(int projectId)
		{
			Project currentProject = projectRepository.GetProjectById(projectId);
			return currentProject.CreationTime;
		}
	}
}
