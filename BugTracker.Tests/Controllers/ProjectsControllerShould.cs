using BugTracker.Controllers;
using BugTracker.Models.Authorization;
using BugTracker.Models.ProjectInvitation;
using BugTracker.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using AuthorizationHelper = BugTracker.Tests.Helpers.AuthorizationHelper;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using BugTracker.Models;
using System.Collections.Generic;
using SmartBreadcrumbs.Nodes;
using BugTracker.Helpers;
using System;
using BugTracker.ViewModels;

namespace BugTracker.Tests.Controllers
{
	public class ProjectsControllerShould
	{
		private readonly Mock<ILogger<ProjectsController>> mockLogger;
		private readonly Mock<IProjectRepository> mockProjectRepo;
		private readonly Mock<IBugReportRepository> mockBugReportRepo;
		private readonly Mock<IActivityRepository> mockActivityRepo;
		private readonly Mock<IAuthorizationService> mockAuthorizationService;
		private readonly Mock<IHttpContextAccessor> mockHttpContextAccessor;
		private readonly Mock<IProjectInviter> mockProjectInviter;
		private readonly Mock<ApplicationUserManager> mockUserManager;
		private readonly Mock<IConfiguration> mockConfiguration;

		private ProjectsController controller;

		public ProjectsControllerShould()
		{
			mockLogger = new Mock<ILogger<ProjectsController>>();
			mockProjectRepo = new Mock<IProjectRepository>();
			mockBugReportRepo = new Mock<IBugReportRepository>();
			mockActivityRepo = new Mock<IActivityRepository>();
			mockAuthorizationService = new Mock<IAuthorizationService>();
			mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
			mockProjectInviter = new Mock<IProjectInviter>();
			mockUserManager = new Mock<ApplicationUserManager>("Fake Connection String");
			mockConfiguration = new Mock<IConfiguration>();

			controller = new ProjectsController(
					mockLogger.Object,
					mockProjectRepo.Object,
					mockBugReportRepo.Object,
					mockActivityRepo.Object,
					mockAuthorizationService.Object,
					mockHttpContextAccessor.Object,
					mockProjectInviter.Object,
					mockUserManager.Object,
					mockConfiguration.Object
				);
		}

		[Fact]
		public async Task Projects_ReturnsZeroResults_IfNotAuthorized()
		{
			AuthorizationHelper.AllowFailure(mockAuthorizationService, mockHttpContextAccessor);

			List<Project> testResults = new List<Project>();
			testResults.Add(new Project { ProjectId = 1, Name = "First test result" });
			testResults.Add(new Project { ProjectId = 2, Name = "Second test result" });
			IEnumerable<Project> enumerableTestResults = testResults;
			mockProjectRepo.Setup(_ => _.GetAll()).Returns(Task.FromResult(enumerableTestResults));

			var result = await controller.Projects();

			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<IEnumerable<Project>>(viewResult.ViewData.Model);
			Assert.Empty(model);
		}

		[Fact]
		public async Task Overview_ReturnsBadRequest_IfIdLessThan1()
		{
			var projectId = 0;
			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor);

			var result = await controller.Overview(projectId);

			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		public async Task Overview_NotFound_IfIdInvalid()
		{
			var projectId = 22;
			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor);
			mockProjectRepo.Setup(_ => _.GetById(projectId)).ThrowsAsync(new InvalidOperationException());

			var result = await controller.Overview(projectId);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task Overview_RedirectToHome_IfNotAuthorized()
		{
			int projectId = 1;
			AuthorizationHelper.AllowFailure(mockAuthorizationService, mockHttpContextAccessor);

			var result = await controller.Overview(projectId);

			Assert.IsType<RedirectToActionResult>(result);
		}

		[Fact]
		public async Task Overview_ReturnsResults_WhenValidProjectId()
		{
			int projectId = 1;
			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor);

			var testProject = new Project();
			mockProjectRepo.Setup(_ => _.GetById(It.IsAny<int>())).Returns(Task.FromResult(testProject));

			List<BugReport> testResults = new List<BugReport>();
			testResults.Add(new BugReport { Title = "First test", ProgramBehaviour = "Test text" });
			testResults.Add(new BugReport { Title = "Second test", ProgramBehaviour = "Test text" });
			IEnumerable<BugReport> enumerableTestResults = testResults;
			mockBugReportRepo.Setup(_ => _.GetAllById(It.IsAny<int>())).Returns(Task.FromResult(enumerableTestResults));

			var result = await controller.Overview(projectId);

			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<OverviewProjectViewModel>(viewResult.ViewData.Model);
			Assert.Equal(2, model.BugReports.Count);
		}
	}
}
