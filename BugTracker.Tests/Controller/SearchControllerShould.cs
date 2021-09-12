using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BugTracker.Controllers;
using BugTracker.Models;
using BugTracker.Repository;
using BugTracker.Repository.Interfaces;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BugTracker.Tests.Controller
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
		public void RedirectToHome_IfProjectIdZero()
		{
			MockHttpSession mockSession = new MockHttpSession();
			mockSession.SetInt32("currentProject", 0);

			var identity = new GenericIdentity("Test user");
			var contextUser = new ClaimsPrincipal(identity);
			var httpContext = new DefaultHttpContext()
			{
				User = contextUser,
				Session = mockSession
			};
			mockContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

			IActionResult actual = controller.Result(viewModel);
			Assert.IsType<RedirectToActionResult>(actual);
		}
	}
}
