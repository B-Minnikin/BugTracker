using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BugTracker.Controllers;
using BugTracker.Models;
using BugTracker.Repository;
using BugTracker.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
		private SearchController controller;

		public SearchControllerShould()
		{
			mockLogger = new Mock<ILogger<SearchController>>();
			mockProjectRepo = new Mock<IProjectRepository>();
			mockBugReportRepo = new Mock<IBugReportRepository>();
			mockSearchRepo = new Mock<ISearchRepository>();
			mockContextAccessor = new Mock<IHttpContextAccessor>();
			mockAuthorizationService = new Mock<IAuthorizationService>();

			controller = new SearchController(
					mockLogger.Object,
					mockProjectRepo.Object,
					mockBugReportRepo.Object,
					mockSearchRepo.Object,
					mockContextAccessor.Object,
					mockAuthorizationService.Object
				);
		}
	}
}
