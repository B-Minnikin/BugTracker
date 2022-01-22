using BugTracker.Controllers;
using BugTracker.Models;
using BugTracker.Models.Authorization;
using BugTracker.Models.Database;
using BugTracker.Repository;
using BugTracker.Repository.Interfaces;
using BugTracker.Tests.Helpers;
using BugTracker.Tests.Mocks;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using AuthorizationHelper = BugTracker.Tests.Helpers.AuthorizationHelper;

namespace BugTracker.Tests.Controllers
{
	public class BugReportControllerShould
	{
		private readonly Mock<ILogger<BugReportController>> mockLogger;
		private readonly Mock<IProjectRepository> mockProjectRepo;
		private readonly Mock<IMilestoneRepository> mockMilestoneRepo;
		private readonly Mock<IBugReportRepository> mockBugReportRepo;
		private readonly Mock<IBugReportStatesRepository> mockBugReportStatesRepo;
		private readonly Mock<IUserSubscriptionsRepository> mockUserSubscriptionsRepo;
		private readonly Mock<IActivityRepository> mockActivityRepo;
		private readonly Mock<ICommentRepository> mockCommentRepo;
		private readonly Mock<IAuthorizationService> mockAuthorizationService;
		private readonly Mock<IHttpContextAccessor> mockHttpContextAccessor;
		private readonly Mock<ISubscriptions> mockSubscriptions;
		private readonly Mock<LinkGenerator> mockLinkGenerator;
		private readonly Mock<UserStore> mockUserStore;
		private readonly Mock<ApplicationUserManager> mockUserManager;
		
		private BugReportController controller;

		public BugReportControllerShould()
		{
			mockLogger = new Mock<ILogger<BugReportController>>();
			mockProjectRepo = new Mock<IProjectRepository>();
			mockMilestoneRepo = new Mock<IMilestoneRepository>();
			mockBugReportRepo = new Mock<IBugReportRepository>();
			mockBugReportStatesRepo = new Mock<IBugReportStatesRepository>();
			mockUserSubscriptionsRepo = new Mock<IUserSubscriptionsRepository>();
			mockActivityRepo = new Mock<IActivityRepository>();
			mockCommentRepo = new Mock<ICommentRepository>();
			mockAuthorizationService = new Mock<IAuthorizationService>();
			mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
			mockSubscriptions = new Mock<ISubscriptions>();
			mockLinkGenerator = new Mock<LinkGenerator>();
			mockUserStore = new Mock<UserStore>("Fake connection string");
			mockUserManager = new Mock<ApplicationUserManager>(mockUserStore.Object, "Fake connection string");

			controller = new BugReportController(
				mockLogger.Object,
				mockProjectRepo.Object,
				mockMilestoneRepo.Object,
				mockBugReportRepo.Object,
				mockBugReportStatesRepo.Object,
				mockUserSubscriptionsRepo.Object,
				mockActivityRepo.Object,
				mockCommentRepo.Object,
				mockAuthorizationService.Object,
				mockHttpContextAccessor.Object,
				mockSubscriptions.Object,
				mockLinkGenerator.Object,
				mockUserManager.Object
			);
		}

		[Fact]
		public async Task CreateReport_Get_ReturnsView_WhenValidSessionProjectId()
		{
			int projectId = 1;
			var project = new Project { ProjectId = projectId, Name = "Test project" };
			var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
			mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor, projectId);

			mockProjectRepo.Setup(_ => _.GetById(It.Is<int>(i => i == projectId))).Returns(Task.FromResult(project)).Verifiable();

			var result = await controller.CreateReport();

			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public async Task CreateReport_Get_ReturnsNotFound_WhenInvalidSessionProjectId()
		{
			int projectId = 0;
			var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
			mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor, projectId);

			var result = await controller.CreateReport();

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task CreateReport_Get_RedirectsToProjectOverview_WhenNotAuthorized()
		{
			int projectId = 1;
			var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId});
			mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

			AuthorizationHelper.AllowFailure(mockAuthorizationService, mockHttpContextAccessor);

			var result = await controller.CreateReport();

			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Overview", redirectToActionResult.ActionName);
			Assert.Equal("Projects", redirectToActionResult.ControllerName);
		}

		[Fact]
		public async Task CreateReport_Post_ReturnsBadRequest_WhenViewModelNull()
		{
			CreateBugReportViewModel viewModel = null;

			int projectId = 1;
			var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
			mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor, projectId);

			var result = await controller.CreateReport(viewModel);

			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		public async Task CreateReport_Post_RedirectsToProjectOverview_WhenNotAuthorized()
		{
			var bugReportViewModel = new CreateBugReportViewModel();

			int projectId = 1;
			var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
			mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

			AuthorizationHelper.AllowFailure(mockAuthorizationService, mockHttpContextAccessor);

			var result = await controller.CreateReport(bugReportViewModel);

			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Overview", redirectToActionResult.ActionName);
			Assert.Equal("Projects", redirectToActionResult.ControllerName);
		}

		[Fact]
		public async Task CreateReport_Post_ReturnsView_WhenModelInvalid()
		{
			var createBugReportViewModel = new CreateBugReportViewModel();

			int projectId = 1;
			var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
			mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor);

			controller.ModelState.AddModelError("Test key", "Error message");
			var result = await controller.CreateReport(createBugReportViewModel);

			var viewResult = Assert.IsType<ViewResult>(result);
			Assert.IsAssignableFrom<CreateBugReportViewModel>(viewResult.ViewData.Model);
		}

		[Fact]
		public async Task CreateReport_Post_ReturnsNotFound_WhenInvalidSessionProjectId()
		{
			var bugReportViewModel = new CreateBugReportViewModel
			{
				BugReport = new BugReport { BugReportId = 1},
				Title = "Test Report",
				ProgramBehaviour = "Behaviour",
				DetailsToReproduce = "Details to reproduce",
				Hidden = false
			};

			int projectId = 0;
			var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
			mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor, projectId);

			var result = await controller.CreateReport(bugReportViewModel);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task CreateReport_Post_RedirectsToReportOverview_WhenModelValid()
		{
			int projectId = 1;
			var bugReport = new BugReport
			{
				BugReportId = 1,
				Title = "Test Report",
				ProgramBehaviour = "Behaviour",
				DetailsToReproduce = "Details to reproduce",
				Hidden = false,
				Severity = Severity.Low,
				Importance = Importance.High,
				ProjectId = projectId
			};
			var createBugReportViewModel = new CreateBugReportViewModel
			{
				Title = bugReport.Title,
				ProgramBehaviour = bugReport.ProgramBehaviour,
				DetailsToReproduce = bugReport.DetailsToReproduce,
				Hidden = bugReport.Hidden,
				Severity = bugReport.Severity,
				Importance = bugReport.Importance,
				Subscribe = true
			};
			var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
			mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor, projectId);

			mockBugReportRepo.Setup(_ => _.Add(It.Is<BugReport>(br => br.ProjectId == bugReport.ProjectId))).Returns(Task.FromResult(bugReport)).Verifiable();
			mockActivityRepo.Setup(_ => _.Add(It.IsAny<ActivityBugReport>())).Verifiable();
			mockBugReportStatesRepo.Setup(_ => _.Add(It.IsAny<BugState>())).Returns(Task.FromResult(new BugState())).Verifiable();
			mockUserSubscriptionsRepo.Setup(_ => _.IsSubscribed(It.Is<int>(i => i > 0), It.Is<int>(br => br == bugReport.BugReportId))).Returns(Task.FromResult(false)).Verifiable();
			mockUserSubscriptionsRepo.Setup(_ => _.AddSubscription(It.Is<int>(i => i > 0), It.Is<int>(br => br == bugReport.BugReportId))).Verifiable();

			var result = await controller.CreateReport(createBugReportViewModel);

			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("ReportOverview", redirectToActionResult.ActionName);
			Assert.True(redirectToActionResult.RouteValues.ContainsKey("id"));
			var routeValueId = redirectToActionResult.RouteValues["id"];
			Assert.Equal(bugReport.BugReportId, routeValueId);
		}

		[Fact]
		public void Subscribe_ReturnsBadRequest_WhenIdLessThan1()
		{
			int bugReportId = 0;
			
			var result = controller.Subscribe(bugReportId);

			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		public void Subscribe_RedirectsToReportOverview_WhenNotAuthorized()
		{
			int bugReportId = 1;
			var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = 1 });
			mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

			AuthorizationHelper.AllowFailure(mockAuthorizationService, mockHttpContextAccessor);

			mockSubscriptions.Setup(_ => _.CreateSubscriptionIfNotSubscribed(It.IsAny<int>(), It.Is<int>(id => id == bugReportId)));

			var result = controller.Subscribe(bugReportId);

			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("ReportOverview", redirectToActionResult.ActionName);

			Assert.True(redirectToActionResult.RouteValues.ContainsKey("id"));
			var routeValueId = redirectToActionResult.RouteValues["id"];
			Assert.Equal(bugReportId, routeValueId);

			// don't create subscription - check
			mockSubscriptions.Verify(_ => _.CreateSubscriptionIfNotSubscribed(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
		}

		[Fact]
		public void Subscribe_ReturnsNotFound_WhenInvalidSessionProjectId()
		{
			int bugReportId = 1;

			int projectId = 0;
			var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
			mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor, projectId);

			var result = controller.Subscribe(bugReportId);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public void Subscribe_RedirectsToReportOverview_AfterSuccessfulSubscribe()
		{
			int bugReportId = 1;
			int projectId = 1;

			var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
			mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor, projectId);

			mockSubscriptions.Setup(_ => _.CreateSubscriptionIfNotSubscribed(It.IsAny<int>(), It.Is<int>(id => id == bugReportId))).Verifiable();

			var result = controller.Subscribe(bugReportId);

			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("ReportOverview", redirectToActionResult.ActionName);
			Assert.True(redirectToActionResult.RouteValues.ContainsKey("id"));
			var routeValueId = redirectToActionResult.RouteValues["id"];
			Assert.Equal(bugReportId, routeValueId);
		}

		[Fact]
		public async Task Edit_Get_ReturnsNotFound_WhenInvalidSessionProjectId()
		{
			int bugReportId = 1;

			int projectId = 0;
			var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
			mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor, projectId);

			var result = await controller.Edit(bugReportId);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task Edit_Get_ReturnsBadRequest_WhenReportIdLessThan1()
		{
			int bugReportId = 0;

			var result = await controller.Edit(bugReportId);

			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		public async Task Edit_Get_RedirectsToProjectsOverview_WhenNotAuthorized()
		{
			int projectId = 1;
			int bugReportId = 1;
			var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
			mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);
			var bugReport = new BugReport { PersonReporting = "Test user"};
			var project = new Project { };

			AuthorizationHelper.AllowFailure(mockAuthorizationService, mockHttpContextAccessor);

			mockBugReportStatesRepo.Setup(_ => _.GetLatestState(It.Is<int>(i => i == bugReportId))).ReturnsAsync(new BugState());
			mockBugReportRepo.Setup(_ => _.GetById(It.Is<int>(i => i == bugReportId))).Returns(Task.FromResult(bugReport));
			mockProjectRepo.Setup(_ => _.GetById(It.Is<int>(i => i == projectId))).Returns(Task.FromResult(project));

			var result = await controller.Edit(bugReportId);

			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Overview", redirectToActionResult.ActionName);
			Assert.Equal("Projects", redirectToActionResult.ControllerName);

			Assert.True(redirectToActionResult.RouteValues.ContainsKey("id"));
			var routeValueId = redirectToActionResult.RouteValues["id"];
			Assert.Equal(projectId, routeValueId);
		}

		[Fact]
		public async Task Edit_Get_ReturnsView_WhenBugReportIdValid()
		{
			int projectId = 1;
			int bugReportId = 1;

			var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
			mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor, projectId);

			var project = new Project { };

			mockProjectRepo.Setup(_ => _.GetById(It.Is<int>(i => i == projectId))).Returns(Task.FromResult(project)).Verifiable();
			mockBugReportStatesRepo.Setup(_ => _.GetLatestState(It.Is<int>(i => i == bugReportId))).Returns(Task.FromResult(new BugState { StateType = StateType.open})).Verifiable();
			mockBugReportRepo.Setup(_ => _.GetById(It.Is<int>(i => i == bugReportId))).Returns(Task.FromResult(new BugReport { PersonReporting = "Test user"})).Verifiable();

			var result = await controller.Edit(bugReportId);

			var viewResult = Assert.IsType<ViewResult>(result);
			Assert.IsType<EditBugReportViewModel>(viewResult.Model);
		}
	}
}
