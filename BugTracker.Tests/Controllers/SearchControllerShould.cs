using System.Collections.Generic;
using System.Security.Claims;
using BugTracker.Controllers;
using BugTracker.Models;
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
using BugTracker.Database.Repository.Interfaces;
using BugTracker.Tests.Helpers;

namespace BugTracker.Tests.Controllers;

public class SearchControllerShould
{
	private readonly Mock<IProjectRepository> mockProjectRepo;
	private readonly Mock<IBugReportRepository> mockBugReportRepo;
	private readonly Mock<ISearchRepository> mockSearchRepo;
	private readonly Mock<IHttpContextAccessor> mockContextAccessor;
	private readonly Mock<IAuthorizationService> mockAuthorizationService;
	private readonly SearchResultsViewModel viewModel;
	private readonly SearchController controller;

	public SearchControllerShould()
	{
		var mockLogger = new Mock<ILogger<SearchController>>();
		mockProjectRepo = new Mock<IProjectRepository>();
		mockBugReportRepo = new Mock<IBugReportRepository>();
		mockSearchRepo = new Mock<ISearchRepository>();
		mockContextAccessor = new Mock<IHttpContextAccessor>();
		mockAuthorizationService = new Mock<IAuthorizationService>();
		var mockLinkGenerator = new Mock<ILinkGenerator>();

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
		const int projectId = 0;
		var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
		mockContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

		var actual = await controller.Result(viewModel);
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

		var actual = await controller.Result(viewModel);
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
		mockProjectRepo.Setup(pr => 
			pr.GetById(It.IsAny<int>())).Returns(Task.FromResult(testProject));

		var testResults = new List<BugReport>
		{
			new() { Title = "First test", ProgramBehaviour = "Test text" },
			new() { Title = "Second test", ProgramBehaviour = "Test text" }
		};
		IEnumerable<BugReport> enumerableTestResults = testResults;
		mockBugReportRepo.Setup(br => br.GetAllById(It.IsAny<int>())).Returns(Task.FromResult(enumerableTestResults));

		var result = await controller.Result(viewModel);
		var viewResult = Assert.IsType<ViewResult>(result);

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
		const int projectId = 0;
		const string query = "admin";

		// Setup authorisation success
		AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockContextAccessor);

		var jsonResult = (JsonResult) await controller.GetProjectMembers(query, projectId);
		var resultList = (List<UserTypeaheadSearchResult>)jsonResult.Value;
		var actual = resultList?.Count;

		const int expected = 0;

		Assert.Equal(expected, actual);
	}

	[Fact]
	public async Task GetProjectMembers_ReturnZeroResults_IfNotAuthorized()
	{
		const int projectId = 2;
		const string query = "admin";

		// Create a single search result from the search repo
		var searchResult = new List<UserTypeaheadSearchResult> { new() { UserName = "Test name", Email = "test@email.com" } };
		IEnumerable<UserTypeaheadSearchResult> enumerableSearchResult = searchResult;
		mockSearchRepo.Setup(s => s.GetMatchingProjectMembersBySearchQuery(query.ToUpper(), projectId)).Returns(Task.FromResult(enumerableSearchResult));
		
		// Setup authorisation failure
		AuthorizationHelper.AllowFailure(mockAuthorizationService, mockContextAccessor);

		var jsonResult = (JsonResult) await controller.GetProjectMembers(query, projectId);
		var actualList = (List<UserTypeaheadSearchResult>)jsonResult.Value;
		var actual = actualList?.Count;

		const int expected = 0;

		Assert.Equal(expected, actual);
	}

	[Fact]
	public async Task GetBugReports_ReturnZeroResults_IfProjectZero()
	{
		const int projectId = 0;
		const string query = "admin";

		// Setup authorisation success
		AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockContextAccessor);

		var jsonResult = (JsonResult) await controller.GetBugReports(query, projectId);
		var resultList = (List<BugReportTypeaheadSearchResult>)jsonResult.Value;
		var actual = resultList?.Count;

		const int expected = 0;

		Assert.Equal(expected, actual);
	}

	[Fact]
	public async Task GetBugReports_ReturnSingleResult_FromLocalId()
	{
		const int projectId = 2;
		const string query = "#1";

		// Setup authorisation success
		AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockContextAccessor);

		// Get a single search result
		var searchResult = new List<BugReportTypeaheadSearchResult>
		{
			new() { BugReportId = 3, LocalBugReportId = 1, Title = "Test Title"}
		};
		IEnumerable<BugReportTypeaheadSearchResult> enumerableSearchResult = searchResult;
		mockSearchRepo.Setup(s => s.GetMatchingBugReportsByLocalIdSearchQuery(1, projectId)).Returns(Task.FromResult(enumerableSearchResult));

		var jsonResult = (JsonResult) await controller.GetBugReports(query, projectId);
		var actualList = (List<BugReportTypeaheadSearchResult>)jsonResult.Value;
		var actualListCount = actualList?.Count;

		const int expectedListCount = 1;

		Assert.Equal(expectedListCount, actualListCount);
	}

	[Fact]
	public async Task GetBugReports_ReturnZeroResults_IfNotAuthorized()
	{
		const int projectId = 2;
		const string query = "admin";

		// Create a single search result from the search repo
		var searchResult = new List<BugReportTypeaheadSearchResult> { new() };
		IEnumerable<BugReportTypeaheadSearchResult> enumerableSearchResult = searchResult;
		mockSearchRepo.Setup(s => s.GetMatchingBugReportsByLocalIdSearchQuery(It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(enumerableSearchResult));
		mockSearchRepo.Setup(s => s.GetMatchingBugReportsByTitleSearchQuery(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(enumerableSearchResult));

		// Setup authorisation failure
		AuthorizationHelper.AllowFailure(mockAuthorizationService, mockContextAccessor);

		var jsonResult = (JsonResult) await controller.GetBugReports(query, projectId);
		var actualList = (List<BugReportTypeaheadSearchResult>)jsonResult.Value;
		var actual = actualList?.Count;

		const int expected = 0;

		Assert.Equal(expected, actual);
	}

	private static void Authorize(Mock<IAuthorizationService> authorizationService, bool willSucceed = true)
	{
		var authorizationResult = willSucceed ? AuthorizationResult.Success() : AuthorizationResult.Failed();
		
		authorizationService.Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(authorizationResult);
	}
}
