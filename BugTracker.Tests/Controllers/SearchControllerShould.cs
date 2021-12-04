using System.Collections.Generic;
using BugTracker.Controllers;
using BugTracker.Models;
using BugTracker.Repository.Interfaces;
using BugTracker.ViewModels;
using BugTracker.Tests.Mocks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System;
using BugTracker.Services;
using System.Threading.Tasks;
using BugTracker.Tests.Helpers;

namespace BugTracker.Tests.Controllers
{
	public class SearchControllerShould
	{
		private readonly Mock<ILogger<SearchController>> mockLogger;
		private readonly Mock<IProjectRepository> mockProjectRepo;
		private readonly Mock<IBugReportRepository> mockBugReportRepo;
		private readonly Mock<ISearchRepository> mockSearchRepo;
		private readonly Mock<IHttpContextAccessor> mockContextAccessor;
		private readonly Mock<IAuthorizationService> mockAuthorizationService;
		private readonly Mock<ILinkGenerator> mockLinkGenerator;
		private readonly SearchResultsViewModel viewModel;
		private SearchController controller;

		public SearchControllerShould()
		{
			mockLogger = new Mock<ILogger<SearchController>>();
			mockProjectRepo = new Mock<IProjectRepository>();
			mockBugReportRepo = new Mock<IBugReportRepository>();
			mockSearchRepo = new Mock<ISearchRepository>();
			mockContextAccessor = new Mock<IHttpContextAccessor>();
			mockAuthorizationService = new Mock<IAuthorizationService>();
			mockLinkGenerator = new Mock<ILinkGenerator>();

			viewModel = new SearchResultsViewModel
			{
				AdvancedSearchResultsBeginCollapsed = true,
				SearchExpression = new SearchExpression
				{
					SearchText = "",
					SearchInDetails = false,
					SearchTitles = true
				},
				SearchResults = new List<BugReport>()
			};

			controller = new SearchController(
					mockLogger.Object,
					mockProjectRepo.Object,
					mockBugReportRepo.Object,
					mockSearchRepo.Object,
					mockContextAccessor.Object,
					mockAuthorizationService.Object,
					mockLinkGenerator.Object
				);
		}

		[Fact]
		public async void Result_RedirectToHome_IfProjectIdZero()
		{
			var projectId = 0;
			var httpContext = MockHttpContextFactory.GetHttpContext(projectId);
			mockContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

			IActionResult actual = await controller.Result(viewModel);
			Assert.IsType<RedirectToActionResult>(actual);
		}

		[Fact]
		public void Result_ThrowException_IfResultsViewModel_IsNull()
		{
			Assert.ThrowsAsync<ArgumentNullException>(() => controller.Result(null));
		}

		[Fact]
		public async void Result_ReturnView_WhenNotAuthorized()
		{
			AuthorizationHelper.AllowFailure(mockAuthorizationService, mockContextAccessor);

			IActionResult actual = await controller.Result(viewModel);
			var viewResult = Assert.IsType<ViewResult>(actual);

			// view model is not passed back to view
			Assert.IsNotType<SearchResultsViewModel>(viewResult.Model);
		}

		[Fact]
		public async Task Result_ReturnView_WhenInvalidModel()
		{
			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockContextAccessor);

			controller.ModelState.AddModelError("x", "Test error");
			viewModel.SearchExpression.SearchText = "Test expression";

			var testProject = new Project();
			mockProjectRepo.Setup(_ => _.GetById(It.IsAny<int>())).Returns(Task.FromResult(testProject));

			List<BugReport> testResults = new List<BugReport>();
			testResults.Add(new BugReport { Title = "First test", ProgramBehaviour = "Test text" });
			testResults.Add(new BugReport { Title = "Second test", ProgramBehaviour = "Test text" });
			IEnumerable<BugReport> enumerableTestResults = testResults;
			mockBugReportRepo.Setup(_ => _.GetAllById(It.IsAny<int>())).Returns(Task.FromResult(enumerableTestResults));

			IActionResult result = await controller.Result(viewModel);
			ViewResult viewResult = Assert.IsType<ViewResult>(result);

			var model = Assert.IsType<SearchResultsViewModel>(viewResult.Model);
			Assert.Equal(viewModel.SearchExpression.SearchText, model.SearchExpression.SearchText);
		}

		[Fact]
		public async Task GetProjectMembers_ThrowException_IfQuery_IsNull()
		{
			await Assert.ThrowsAsync<ArgumentNullException>(() => controller.GetProjectMembers(null, 1));
		}

		[Fact]
		public async Task GetProjectMembers_ReturnZeroResults_IfProjectZero()
		{
			int projectId = 0;
			string query = "admin";

			// Setup authorisation success
			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockContextAccessor);

			var jsonResult = (JsonResult) await controller.GetProjectMembers(query, projectId);
			var resultList = (List<UserTypeaheadSearchResult>)jsonResult.Value;
			int actual = resultList.Count;

			var expected = 0;

			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task GetProjectMembers_ReturnZeroResults_IfNotAuthorized()
		{
			int projectId = 2;
			string query = "admin";

			// Create a single search result from the search repo
			List<UserTypeaheadSearchResult> searchResult = new List<UserTypeaheadSearchResult>();
			searchResult.Add(new UserTypeaheadSearchResult { UserName = "Test name", Email = "test@email.com" });
			IEnumerable<UserTypeaheadSearchResult> enumerableSearchResult = searchResult;
			mockSearchRepo.Setup(_ => _.GetMatchingProjectMembersBySearchQuery(query.ToUpper(), projectId)).Returns(Task.FromResult(enumerableSearchResult));

			// Setup authorisation failure
			AuthorizationHelper.AllowFailure(mockAuthorizationService, mockContextAccessor);

			var jsonResult = (JsonResult) await controller.GetProjectMembers(query, projectId);
			var actualList = (List<UserTypeaheadSearchResult>)jsonResult.Value;
			int actual = actualList.Count;

			var expected = 0;

			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task GetBugReports_ReturnZeroResults_IfProjectZero()
		{
			int projectId = 0;
			string query = "admin";

			// Setup authorisation success
			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockContextAccessor);

			var jsonResult = (JsonResult) await controller.GetBugReports(query, projectId);
			var resultList = (List<BugReportTypeaheadSearchResult>)jsonResult.Value;
			int actual = resultList.Count;

			var expected = 0;

			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task GetBugReports_ReturnSingleResult_FromLocalId()
		{
			int projectId = 2;
			string query = "#1";

			// Setup authorisation success
			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockContextAccessor);

			// Get a single search result
			List<BugReportTypeaheadSearchResult> searchResult = new List<BugReportTypeaheadSearchResult>();
			searchResult.Add(new BugReportTypeaheadSearchResult { BugReportId = 3, LocalBugReportId = 1, Title = "Test Title"});
			IEnumerable<BugReportTypeaheadSearchResult> enumerableSearchResult = searchResult;
			mockSearchRepo.Setup(_ => _.GetMatchingBugReportsByLocalIdSearchQuery(1, projectId)).Returns(Task.FromResult(enumerableSearchResult));

			var jsonResult = (JsonResult) await controller.GetBugReports(query, projectId);
			var actualList = (List<BugReportTypeaheadSearchResult>)jsonResult.Value;
			int actualListCount = actualList.Count;

			var expectedListCount = 1;

			Assert.Equal(expectedListCount, actualListCount);
		}

		[Fact]
		public async Task GetBugReports_ReturnZeroResults_IfNotAuthorized()
		{
			int projectId = 2;
			string query = "admin";

			// Create a single search result from the search repo
			List<BugReportTypeaheadSearchResult> searchResult = new List<BugReportTypeaheadSearchResult>();
			searchResult.Add(new BugReportTypeaheadSearchResult());
			IEnumerable<BugReportTypeaheadSearchResult> enumerableSearchResult = searchResult;
			mockSearchRepo.Setup(_ => _.GetMatchingBugReportsByLocalIdSearchQuery(It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(enumerableSearchResult));
			mockSearchRepo.Setup(_ => _.GetMatchingBugReportsByTitleSearchQuery(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(enumerableSearchResult));

			// Setup authorisation failure
			AuthorizationHelper.AllowFailure(mockAuthorizationService, mockContextAccessor);

			var jsonResult = (JsonResult) await controller.GetBugReports(query, projectId);
			var actualList = (List<BugReportTypeaheadSearchResult>)jsonResult.Value;
			int actual = actualList.Count;

			var expected = 0;

			Assert.Equal(expected, actual);
		}			
	}
}
