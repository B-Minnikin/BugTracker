using System.Collections.Generic;
using System.Security.Claims;
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
					mockAuthorizationService.Object
				);
		}

		[Fact]
		public void Result_RedirectToHome_IfProjectIdZero()
		{
			var projectId = 0;
			var httpContext = MockHttpContextFactory.GetHttpContext(projectId);
			mockContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

			IActionResult actual = controller.Result(viewModel);
			Assert.IsType<RedirectToActionResult>(actual);
		}

		[Fact]
		public void Result_ThrowException_IfResultsViewModel_IsNull()
		{
			Assert.Throws<ArgumentNullException>(() => controller.Result(null));
		}

		[Fact]
		public void Result_ReturnView_WhenNotAuthorized()
		{
			var projectId = 1;
			var userName = "Test User";
			var httpContext = MockHttpContextFactory.GetHttpContext(projectId, userName);
			mockContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

			// force the authorization failure
			mockAuthorizationService.Setup(_ => _.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(AuthorizationResult.Failed);

			IActionResult actual = controller.Result(viewModel);
			var viewResult = Assert.IsType<ViewResult>(actual);

			// view model is not passed back to view
			Assert.IsNotType<SearchResultsViewModel>(viewResult.Model);
		}

		[Fact]
		public void Result_ReturnView_WhenInvalidModel()
		{
			var httpContext = MockHttpContextFactory.GetHttpContext();
			mockContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);
			mockAuthorizationService.Setup(_ => _.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(AuthorizationResult.Success);

			controller.ModelState.AddModelError("x", "Test error");
			viewModel.SearchExpression.SearchText = "Test expression";

			Project testProject = new Project();
			mockProjectRepo.Setup(_ => _.GetById(It.IsAny<int>())).Returns(testProject);

			List<BugReport> testResults = new List<BugReport>();
			testResults.Add(new BugReport { Title = "First test", ProgramBehaviour = "Test text" });
			testResults.Add(new BugReport { Title = "Second test", ProgramBehaviour = "Test text" });
			mockBugReportRepo.Setup(_ => _.GetAllById(It.IsAny<int>())).Returns(testResults);

			IActionResult result = controller.Result(viewModel);
			ViewResult viewResult = Assert.IsType<ViewResult>(result);

			var model = Assert.IsType<SearchResultsViewModel>(viewResult.Model);
			Assert.Equal(viewModel.SearchExpression.SearchText, model.SearchExpression.SearchText);
		}
	}
}
