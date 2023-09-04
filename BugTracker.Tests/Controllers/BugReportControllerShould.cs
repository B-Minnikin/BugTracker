using BugTracker.Controllers;
using BugTracker.Models;
using BugTracker.Models.Authorization;
using BugTracker.Repository;
using BugTracker.Repository.Interfaces;
using BugTracker.Services;
using BugTracker.Tests.Helpers;
using BugTracker.Tests.Mocks;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using BugTracker.Database.Context;
using BugTracker.Models.Subscription;
using Xunit;
using AuthorizationHelper = BugTracker.Tests.Helpers.AuthorizationHelper;

namespace BugTracker.Tests.Controllers;

public class BugReportControllerShould
{
	private readonly Mock<IProjectRepository> mockProjectRepo;
	private readonly Mock<IBugReportRepository> mockBugReportRepo;
	private readonly Mock<IBugReportStatesRepository> mockBugReportStatesRepo;
	private readonly Mock<IUserSubscriptionsRepository> mockUserSubscriptionsRepo;
	private readonly Mock<IActivityRepository> mockActivityRepo;
	private readonly Mock<IAuthorizationService> mockAuthorizationService;
	private readonly Mock<IHttpContextAccessor> mockHttpContextAccessor;
	private readonly Mock<ISubscriptions> mockSubscriptions;
	private readonly Mock<ApplicationUserManager> mockUserManager;
	
	private BugReportController controller;

	public BugReportControllerShould()
	{
		mockProjectRepo = new Mock<IProjectRepository>();
		var mockMilestoneRepo = new Mock<IMilestoneRepository>();
		mockBugReportRepo = new Mock<IBugReportRepository>();
		mockBugReportStatesRepo = new Mock<IBugReportStatesRepository>();
		mockUserSubscriptionsRepo = new Mock<IUserSubscriptionsRepository>();
		mockActivityRepo = new Mock<IActivityRepository>();
		var mockCommentRepo = new Mock<ICommentRepository>();
		mockAuthorizationService = new Mock<IAuthorizationService>();
		mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
		mockSubscriptions = new Mock<ISubscriptions>();
		var mockLinkGenerator = new Mock<LinkGenerator>();
		var mockUserStore = new Mock<UserStore>("Fake connection string");
		mockUserManager = new Mock<ApplicationUserManager>(mockUserStore.Object, "Fake connection string");

		controller = new BugReportController(
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

		mockProjectRepo.Setup(p => p.GetById(It.Is<int>(i => i == projectId))).Returns(Task.FromResult(project)).Verifiable();

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
		const int projectId = 1;
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

		AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor);

		mockBugReportRepo.Setup(brr => brr.Add(It.Is<BugReport>(br => br.ProjectId == bugReport.ProjectId))).Returns(Task.FromResult(bugReport)).Verifiable();
		mockActivityRepo.Setup(a => a.Add(It.IsAny<ActivityBugReport>())).Verifiable();
		mockBugReportStatesRepo.Setup(br => br.Add(It.IsAny<BugState>())).Returns(Task.FromResult(new BugState())).Verifiable();
		mockUserSubscriptionsRepo.Setup(us => us.IsSubscribed(It.IsAny<string>(), It.Is<int>(br => br == bugReport.BugReportId))).Returns(Task.FromResult(false)).Verifiable();
		mockUserSubscriptionsRepo.Setup(us => us.AddSubscription(It.IsAny<string>(), It.Is<int>(br => br == bugReport.BugReportId))).Verifiable();

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

		mockSubscriptions.Setup(s => s.CreateSubscriptionIfNotSubscribed(It.IsAny<string>(), It.Is<int>(id => id == bugReportId)));

		var result = controller.Subscribe(bugReportId);

		var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("ReportOverview", redirectToActionResult.ActionName);

		Assert.True(redirectToActionResult.RouteValues.ContainsKey("id"));
		var routeValueId = redirectToActionResult.RouteValues["id"];
		Assert.Equal(bugReportId, routeValueId);

		// don't create subscription - check
		mockSubscriptions.Verify(s => s.CreateSubscriptionIfNotSubscribed(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
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

		mockSubscriptions.Setup(s => s.CreateSubscriptionIfNotSubscribed(It.IsAny<string>(), It.Is<int>(id => id == bugReportId))).Verifiable();

		var result = controller.Subscribe(bugReportId);

		var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("ReportOverview", redirectToActionResult.ActionName);
		Assert.True(redirectToActionResult.RouteValues?.ContainsKey("id"));
		var routeValueId = redirectToActionResult.RouteValues?["id"];
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
		var project = new Project();

		AuthorizationHelper.AllowFailure(mockAuthorizationService, mockHttpContextAccessor);

		mockBugReportStatesRepo.Setup(brs => brs.GetLatestState(It.Is<int>(i => i == bugReportId))).ReturnsAsync(new BugState());
		mockBugReportRepo.Setup(br => br.GetById(It.Is<int>(i => i == bugReportId))).Returns(Task.FromResult(bugReport));
		mockProjectRepo.Setup(p => p.GetById(It.Is<int>(i => i == projectId))).Returns(Task.FromResult(project));

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

		var project = new Project();

		mockProjectRepo.Setup(p => p.GetById(It.Is<int>(i => i == projectId))).Returns(Task.FromResult(project)).Verifiable();
		mockBugReportStatesRepo.Setup(brs => brs.GetLatestState(It.Is<int>(i => i == bugReportId))).Returns(Task.FromResult(new BugState { StateType = StateType.Open})).Verifiable();
		mockBugReportRepo.Setup(br => br.GetById(It.Is<int>(i => i == bugReportId))).Returns(Task.FromResult(new BugReport { PersonReporting = "Test user"})).Verifiable();

		var result = await controller.Edit(bugReportId);

		var viewResult = Assert.IsType<ViewResult>(result);
		Assert.IsType<EditBugReportViewModel>(viewResult.Model);
	}

	[Fact]
	public async Task Edit_Post_ReturnsBadRequest_WhenViewModelNull()
	{
		var result = await controller.Edit(null);

		Assert.IsType<BadRequestResult>(result);
	}

	[Fact]
	public async Task Edit_Post_ReturnsNotFound_WhenInvalidSessionProjectId()
	{
		int projectId = 0;
		int bugReportId = 1;
		var bugReport = new BugReport { BugReportId = bugReportId, PersonReporting = "Test user" };
		var viewModel = new EditBugReportViewModel
		{
			BugReport = bugReport,
			CurrentState = StateType.Open
		};

		var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
		mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

		AuthorizationHelper.AllowFailure(mockAuthorizationService, mockHttpContextAccessor, projectId);

		var result = await controller.Edit(viewModel);

		Assert.IsType<NotFoundResult>(result);
	}

	[Fact]
	public async Task Edit_Post_RedirectToReportOverview_WhenNotAuthorized()
	{
		int projectId = 1;
		var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
		mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

		AuthorizationHelper.AllowFailure(mockAuthorizationService, mockHttpContextAccessor);

		int bugReportId = 1;
		var bugReport = new BugReport { BugReportId = bugReportId, PersonReporting = "Test user" };
		var viewModel = new EditBugReportViewModel
		{
			BugReport = bugReport,
			CurrentState = StateType.Open
		};

		var result = await controller.Edit(viewModel);

		var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("ReportOverview", redirectToActionResult.ActionName);

		Assert.True(redirectToActionResult.RouteValues.ContainsKey("id"));
		var routeValueId = redirectToActionResult.RouteValues["id"];
		Assert.Equal(bugReportId, routeValueId);
	}

	[Fact]
	public async Task Edit_Post_ReturnView_WhenModelInvalid()
	{
		int projectId = 1;
		var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
		mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

		AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor);

		int bugReportId = 1;
		var bugReport = new BugReport { BugReportId = bugReportId, PersonReporting = "Test user" };
		var viewModel = new EditBugReportViewModel
		{
			BugReport = bugReport,
			CurrentState = StateType.Open
		};

		controller.ModelState.AddModelError("Test key", "Error message");
		var result = await controller.Edit(viewModel);

		var viewResult = Assert.IsType<ViewResult>(result);
		Assert.IsAssignableFrom<EditBugReportViewModel>(viewResult.ViewData.Model);
		var viewModelResult = viewResult.ViewData.Model as EditBugReportViewModel;
		Assert.Equal(viewModelResult.BugReport.BugReportId, bugReportId);
	}

	[Fact]
	public async Task Edit_Post_RedirectToReportOverview_WhenModelValid()
	{
		int projectId = 1;
		var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
		mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

		AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor);

		int bugReportId = 1;
		var bugReport = new BugReport { 
			BugReportId = bugReportId,
			Title = "Test title",
			DetailsToReproduce = "Details to reproduce",
			ProgramBehaviour = "Program behaviour",
			Severity = Severity.Medium,
			Importance = Importance.Medium,
			CreationTime = DateTime.Now,
			Hidden = false,
			PersonReporting = "Test user" 
		};
		var viewModel = new EditBugReportViewModel
		{
			BugReport = bugReport,
			CurrentState = StateType.Open
		};
		var bugState = new BugState
		{
			BugStateId = 1,
			BugReportId = bugReportId
		};

		mockBugReportRepo.Setup(br => br.GetById(It.Is<int>(i => i == bugReportId))).Returns(Task.FromResult(bugReport)).Verifiable();
		mockBugReportRepo.Setup(br => br.Update(It.IsAny<BugReport>())).Returns(Task.FromResult(bugReport));
		mockActivityRepo.Setup(a => a.Add(It.IsAny<ActivityBugReport>())).Returns(Task.FromResult(new ActivityBugReport() as Activity));
		mockBugReportStatesRepo.Setup(brs => brs.GetLatestState(It.Is<int>(i => i == bugReportId))).Returns(Task.FromResult(new BugState { StateType = StateType.Closed}));
		mockBugReportStatesRepo.Setup(brs => brs.Add(It.IsAny<BugState>())).Returns(Task.FromResult(bugState)).Verifiable();
		mockSubscriptions.Setup(s => s.NotifyBugReportStateChanged(It.Is<BugState>(bs => bs.Equals(bugState)), It.IsAny<ApplicationLinkGenerator>(), It.Is<int>(i => i == bugReportId))).Verifiable();
		mockActivityRepo.Setup(a => a.Add(It.IsAny<ActivityBugReportStateChange>())).Returns(Task.FromResult(new ActivityBugReportStateChange() as Activity)).Verifiable();

		var result = await controller.Edit(viewModel);

		var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("ReportOverview", redirectToActionResult.ActionName);

		Assert.True(redirectToActionResult.RouteValues.ContainsKey("id"));
		var routeValueId = redirectToActionResult.RouteValues["id"];
		Assert.Equal(bugReportId, routeValueId);
	}

	[Fact]
	public async Task Delete_ReturnsBadRequest_WhenBugReportIdLessThan1()
	{
		int bugReportId = 0;

		var result = await controller.Delete(bugReportId);

		Assert.IsType<BadRequestResult>(result);
	}

	[Fact]
	public async Task Delete_ReturnsNotFound_WhenInvalidSessionProjectId()
	{
		int projectId = 0;
		int bugReportId = 1;
		var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
		mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

		AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor, projectId);

		var result = await controller.Delete(bugReportId);

		Assert.IsType<NotFoundResult>(result);
	}

	[Fact]
	public async Task Delete_RedirectsToProjectsOverview_WhenNotAuthorized()
	{
		int projectId = 1;
		var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
		mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

		AuthorizationHelper.AllowFailure(mockAuthorizationService, mockHttpContextAccessor);

		int bugReportId = 1;
		var bugReport = new BugReport { BugReportId = bugReportId, PersonReporting = "Test user" };
		
		mockBugReportRepo.Setup(br => br.GetById(It.Is<int>(i => i == bugReportId))).Returns(Task.FromResult(bugReport)).Verifiable();

		var result = await controller.Delete(bugReportId);

		mockBugReportRepo.Verify(br => br.Delete(It.Is<int>(i => i == bugReportId)), Times.Never);

		var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("Overview", redirectToActionResult.ActionName);
		Assert.Equal("Projects", redirectToActionResult.ControllerName);

		Assert.True(redirectToActionResult.RouteValues.ContainsKey("id"));
		var routeValueId = redirectToActionResult.RouteValues["id"];
		Assert.Equal(projectId, routeValueId);
	}

	[Fact]
	public async Task Delete_RedirectsToProjectsOverview_WhenAuthorized()
	{
		// verifiable delete

		int projectId = 1;
		var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
		mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

		AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor);

		int bugReportId = 1;
		var bugReport = new BugReport { BugReportId = bugReportId, PersonReporting = "Test user" };

		mockBugReportRepo.Setup(br => br.GetById(It.Is<int>(i => i == bugReportId))).Returns(Task.FromResult(bugReport)).Verifiable();
		mockBugReportRepo.Setup(br => br.Delete(It.Is<int>(i => i == bugReportId))).Returns(Task.FromResult(bugReport)).Verifiable();

		var result = await controller.Delete(bugReportId);

		var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("Overview", redirectToActionResult.ActionName);
		Assert.Equal("Projects", redirectToActionResult.ControllerName);

		Assert.True(redirectToActionResult.RouteValues.ContainsKey("id"));
		var routeValueId = redirectToActionResult.RouteValues["id"];
		Assert.Equal(projectId, routeValueId);
	}

	[Fact]
	public async Task AssignMember_Get_ReturnsNotFound_WhenInvalidSessionProjectId()
	{
		int projectId = 0;
		int bugReportId = 1;
		var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
		mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

		AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor, projectId);
		mockBugReportRepo.Setup(br => br.GetById(It.Is<int>(i => i == bugReportId))).Returns(Task.FromResult(new BugReport())).Verifiable();

		var result = await controller.AssignMember(bugReportId);

		Assert.IsType<NotFoundResult>(result);
	}

	[Fact]
	public async Task AssignMember_Get_ReturnsBadRequest_WhenBugReportIdLessThan1()
	{
		const int bugReportId = 0;

		var result = await controller.AssignMember(bugReportId);

		Assert.IsType<BadRequestResult>(result);
	}

	[Fact]
	public async Task AssignMember_Get_RedirectsToHome_WhenNotAuthorized()
	{
		const int projectId = 1;
		var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
		mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

		AuthorizationHelper.AllowFailure(mockAuthorizationService, mockHttpContextAccessor);

		const int bugReportId = 1;
		var bugReport = new BugReport { BugReportId = bugReportId, PersonReporting = "Test user" };
		mockBugReportRepo.Setup(br => br.GetById(It.Is<int>(i => i == bugReportId))).Returns(Task.FromResult(bugReport)).Verifiable();

		var result = await controller.AssignMember(bugReportId);

		mockProjectRepo.Verify(p => p.GetById(It.IsAny<int>()), Times.Never);

		var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("Index", redirectToActionResult.ActionName);
		Assert.Equal("Home", redirectToActionResult.ControllerName);
	}

	[Fact]
	public async Task AssignMember_Get_ReturnsView_WhenAuthorized()
	{
		const int projectId = 1;
		var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
		mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

		AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor);

		const int bugReportId = 1;
		var bugReport = new BugReport { BugReportId = bugReportId, PersonReporting = "Test user" };
		var project = new Project();
		var usersList = new List<ApplicationUser>();
		IEnumerable<ApplicationUser> users = usersList;

		mockBugReportRepo.Setup(br => br.GetById(It.Is<int>(i => i == bugReportId))).Returns(Task.FromResult(bugReport)).Verifiable();
		mockProjectRepo.Setup(p => p.GetById(It.Is<int>(i => i == projectId))).Returns(Task.FromResult(project));
		mockBugReportRepo.Setup(br => br.GetAssignedUsersForBugReport(It.Is<int>(i => i == bugReportId))).Returns(Task.FromResult(users));

		var result = await controller.AssignMember(bugReportId);

		var viewResult = Assert.IsType<ViewResult>(result);
		Assert.IsAssignableFrom<AssignMemberViewModel>(viewResult.ViewData.Model);
		var viewModelResult = viewResult.ViewData.Model as AssignMemberViewModel;
		Assert.Equal(viewModelResult?.BugReportId, bugReportId);
		Assert.Equal(viewModelResult?.ProjectId, projectId);
	}

	[Fact]
	public async Task AssignMember_Post_ReturnsNotFound_WhenInvalidSessionProjectId()
	{
		const int projectId = 0;
		var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId, UserId = "2", UserName = "Test User" });
		mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);
		AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor, projectId);

		const int bugReportId = 1;
		var viewModel = new AssignMemberViewModel { ProjectId = projectId, BugReportId = bugReportId };

		const int renameUserId = 1;
		var assignedUser = new ApplicationUser() { UserName = "Test Assignee User", Id = renameUserId.ToString() };
		mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult(assignedUser));
		mockBugReportRepo.Setup(br => br.AddUserAssignedToBugReport(It.IsAny<string>(), It.Is<int>(i => i == bugReportId))).Verifiable();
		
		var result = await controller.AssignMember(viewModel);

		Assert.IsType<NotFoundResult>(result);
	}

	[Fact]
	[SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
	public async Task AssignMember_Post_ReturnsBadRequest_WhenViewModelNull()
	{
		AssignMemberViewModel viewModel = null;

		var result = await controller.AssignMember(viewModel);

		Assert.IsType<BadRequestResult>(result);
	}

	[Fact]
	public async Task AssignMember_Post_RedirectsToHome_WhenNotAuthorized()
	{
		var assignMemberViewModel = new AssignMemberViewModel();

		const int projectId = 1;
		var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
		mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

		AuthorizationHelper.AllowFailure(mockAuthorizationService, mockHttpContextAccessor);

		var result = await controller.AssignMember(assignMemberViewModel);

		var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("Index", redirectToActionResult.ActionName);
		Assert.Equal("Home", redirectToActionResult.ControllerName);
	}

	[Fact]
	public async Task AssignMember_Post_RedirectsToAssignMember_WhenAuthorized()
	{
		const int projectId = 1;
		var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId, UserId = "2", UserName = "Test User" });
		mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);
		AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor);

		const int bugReportId = 1;
		var viewModel = new AssignMemberViewModel { ProjectId = projectId, BugReportId = bugReportId };

		const int renameUserId = 1;
		var assignedUser = new ApplicationUser() { UserName = "Test Assignee User", Id = renameUserId.ToString() };
		mockUserManager.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult(assignedUser));
		mockBugReportRepo.Setup(br => br.AddUserAssignedToBugReport(It.IsAny<string>(), It.Is<int>(i => i == bugReportId))).Verifiable();

		mockActivityRepo.Setup(a => a.Add(It.IsAny<ActivityBugReportAssigned>())).Returns(Task.FromResult(new ActivityBugReportAssigned() as Activity)).Verifiable();

		var result = await controller.AssignMember(viewModel);

		var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("AssignMember", redirectToActionResult.ActionName);

		Assert.True(redirectToActionResult?.RouteValues?.ContainsKey("bugReportId"));
		var routeValueId = redirectToActionResult.RouteValues["bugReportId"];
		Assert.Equal(bugReportId, routeValueId);
	}

	[Fact]
	public async Task RemoveAssignedMember_Post_RedirectsToAssignMember_WhenAuthorized()
	{
		const int bugReportId = 1;
		const string memberEmail = "member@email.com";
		const int projectId = 1;
		const int userId = 2;

		var user = new ApplicationUser()
		{
			Id = userId.ToString()
		};

		var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId, UserId = "2", UserName = "Test User" });
		mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);
		AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor);

		mockUserManager.Setup(um => um.FindByEmailAsync(It.Is<string>(s => s == memberEmail))).Returns(Task.FromResult(user));
		mockBugReportRepo.Setup(br => br.DeleteUserAssignedToBugReport(It.IsAny<string>(), It.Is<int>(i => i == bugReportId))).Verifiable();

		var result = await controller.RemoveAssignedMember(projectId, bugReportId, memberEmail);

		var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("AssignMember", redirectToActionResult.ActionName);

		Assert.True(redirectToActionResult.RouteValues.ContainsKey("bugReportId"));
		var routeValueId = redirectToActionResult.RouteValues["bugReportId"];
		Assert.Equal(bugReportId, routeValueId);
	}

	[Fact]
	public async Task RemoveAssignedMember_Post_RedirectsToHome_WhenNotAuthorized()
	{
		const int projectId = 1;
		const int bugReportId = 1;
		const string memberEmail = "member@email.com";

		var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
		mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);
		AuthorizationHelper.AllowFailure(mockAuthorizationService, mockHttpContextAccessor);

		var result = await controller.RemoveAssignedMember(projectId, bugReportId, memberEmail);

		var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("Index", redirectToActionResult.ActionName);
		Assert.Equal("Home", redirectToActionResult.ControllerName);
	}

	[Fact]
	public async Task RemoveAssignedMember_Post_ReturnsBadRequest_WhenBugReportIdLessThan1()
	{
		const int projectId = 1;
		const int bugReportId = 0;
		const string memberEmail = "member@email";

		var result = await controller.RemoveAssignedMember(projectId, bugReportId, memberEmail);

		Assert.IsType<BadRequestResult>(result);
	}

	[Fact]
	public async Task RemoveAssignedMember_Post_ReturnsBadRequest_WhenProjectIdLessThan1()
	{
		const int projectId = 0;
		const int bugReportId = 1;
		const string memberEmail = "member@email";

		var result = await controller.RemoveAssignedMember(projectId, bugReportId, memberEmail);

		Assert.IsType<BadRequestResult>(result);
	}

	[Fact]
	public async Task RemoveAssignedMember_Post_ReturnsBadRequest_WhenEmailEmptyOrNull()
	{
		const int projectId = 1;
		const int bugReportId = 1;
		const string memberEmail = "";

		var result = await controller.RemoveAssignedMember(projectId, bugReportId, memberEmail);

		Assert.IsType<BadRequestResult>(result);
	}

	[Fact]
	public async Task ManageLinks_ReturnsBadRequest_WhenBugReportIdLessThan1()
	{
		const int bugReportId = 0;

		var result = await controller.ManageLinks(bugReportId);

		Assert.IsType<BadRequestResult>(result);
	}

	[Fact]
	public async Task ManageLinks_ReturnsNotFound_WhenInvalidSessionProjectId()
	{
		const int projectId = 0;
		const int bugReportId = 1;
		var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId, UserId = "2", UserName = "Test User" });
		mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

		var result = await controller.ManageLinks(bugReportId);

		Assert.IsType<NotFoundResult>(result);
	}

	[Fact]
	public async Task ManageLinks_RedirectsToHome_WhenNotAuthorized()
	{
		const int projectId = 1;
		const int bugReportId = 1;

		var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
		mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);
		AuthorizationHelper.AllowFailure(mockAuthorizationService, mockHttpContextAccessor);

		var result = await controller.ManageLinks(bugReportId);

		var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("Index", redirectToActionResult.ActionName);
		Assert.Equal("Home", redirectToActionResult.ControllerName);
	}

	[Fact]
	public async Task ManageLinks_ReturnsModel_WhenAuthorized()
	{
		const int bugReportId = 1;
		const int projectId = 1;

		var project = new Project
		{
			ProjectId = projectId
		};

		IEnumerable<BugReport> linkedReports = new List<BugReport>
		{
			new BugReport{ BugReportId = 2, Title = "First report" },
			new BugReport{ BugReportId = 3, Title = "Second report" }
		};

		var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId, UserId = "2", UserName = "Test User" });
		mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);
		AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor);

		mockBugReportRepo.Setup(br => br.GetById(It.Is<int>(i => i == bugReportId))).Returns(Task.FromResult(new BugReport { BugReportId = 4, Title = "Third report" }));
		mockProjectRepo.Setup(p => p.GetById(It.Is<int>(i => i == projectId))).Returns(Task.FromResult(project));
		mockBugReportRepo.Setup(br => br.GetLinkedReports(It.Is<int>(i => i == bugReportId))).Returns(Task.FromResult(linkedReports));

		var result = await controller.ManageLinks(bugReportId);

		var viewResult = Assert.IsType<ViewResult>(result);
		Assert.IsAssignableFrom<ManageLinksViewModel>(viewResult.ViewData.Model);
		var viewModelResult = viewResult.ViewData.Model as ManageLinksViewModel;
		Assert.Equal(viewModelResult?.BugReportId, bugReportId);
		Assert.Equal(viewModelResult?.ProjectId, projectId);
	}

	[Fact]
	public async Task LinkReports_ReturnsBadRequest_WhenViewModelNull()
	{
		var result = await controller.LinkReports(null);

		Assert.IsType<BadRequestResult>(result);
	}

	[Fact]
	public async Task LinkReports_RedirectsToHome_WhenNotAuthorized()
	{
		const int projectId = 1;
		var model = new LinkReportsViewModel();

		var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
		mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);
		AuthorizationHelper.AllowFailure(mockAuthorizationService, mockHttpContextAccessor);

		var result = await controller.LinkReports(model);

		var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("Index", redirectToActionResult.ActionName);
		Assert.Equal("Home", redirectToActionResult.ControllerName);
	}

	[Fact]
	public async Task LinkReports_RedirectsToReportOverview_WhenAuthorized()
	{
		const int bugReportId = 1;
		const int localBugReportId = 2;
		const int projectId = 1;

		var model = new LinkReportsViewModel { BugReportId = 3, LinkToBugReportLocalId = localBugReportId, ProjectId = projectId};

		var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId, UserId = "2", UserName = "Test User" });
		mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);
		AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor);

		var linkToReport = new BugReport { BugReportId = bugReportId };
		mockBugReportRepo.Setup(br => br.GetBugReportByLocalId(It.Is<int>(i => i == model.LinkToBugReportLocalId), It.Is<int>(i => i == projectId))).Returns(Task.FromResult(linkToReport));
		mockBugReportRepo.Setup(br => br.AddBugReportLink(It.Is<int>(i => i == model.BugReportId), It.Is<int>(i => i == linkToReport.BugReportId))).Verifiable();
		mockActivityRepo.Setup(a => a.Add(It.IsAny<ActivityBugReportLink>())).Verifiable();

		var result = await controller.LinkReports(model);
		Assert.NotNull(result);

		var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("ReportOverview", redirectToActionResult.ActionName);

		Assert.True(redirectToActionResult.RouteValues.ContainsKey("id"));
		var routeValueId = redirectToActionResult.RouteValues["id"];
		Assert.Equal(model.BugReportId, routeValueId);
	}

	[Fact]
	public async Task LinkReports_ReturnsNotFound_WhenInvalidSessionProjectId()
	{
		const int projectId = 0;
		const int bugReportId = 1;
		const int localBugReportId = 2;
		var model = new LinkReportsViewModel { BugReportId = bugReportId, LinkToBugReportLocalId = localBugReportId, ProjectId = projectId };

		var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId, UserId = "2", UserName = "Test User" });
		mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

		var result = await controller.LinkReports(model);

		Assert.IsType<NotFoundResult>(result);
	}

	[Fact]
	public async Task DeleteLink_ReturnsBadRequest_WhenProjectIdLessThan1()
	{
		const int projectId = 0;
		const int bugReportId = 1;
		const int linkToBugReportId = 2;

		var result = await controller.DeleteLink(projectId, bugReportId, linkToBugReportId);

		Assert.IsType<BadRequestResult>(result);
	}

	[Fact]
	public async Task DeleteLink_ReturnsBadRequest_WhenBugReportIdLessThan1()
	{
		const int projectId = 1;
		const int bugReportId = 0;
		const int linkToBugReportId = 2;

		var result = await controller.DeleteLink(projectId, bugReportId, linkToBugReportId);

		Assert.IsType<BadRequestResult>(result);
	}

	[Fact]
	public async Task DeleteLink_ReturnsBadRequest_WhenLinkToBugReportIdLessThan1()
	{
		const int projectId = 1;
		const int bugReportId = 1;
		const int linkToBugReportId = 0;

		var result = await controller.DeleteLink(projectId, bugReportId, linkToBugReportId);

		Assert.IsType<BadRequestResult>(result);
	}

	[Fact]
	public async Task DeleteLink_RedirectsToHome_WhenNotAuthorized()
	{
		const int projectId = 1;
		const int bugReportId = 1;
		const int linkToBugReportId = 1;

		var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
		mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);
		AuthorizationHelper.AllowFailure(mockAuthorizationService, mockHttpContextAccessor);

		var result = await controller.DeleteLink(projectId, bugReportId, linkToBugReportId);

		var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("Index", redirectToActionResult.ActionName);
		Assert.Equal("Home", redirectToActionResult.ControllerName);
	}

	[Fact]
	public async Task DeleteLink_RedirectsToReportOverview_WhenAuthorized()
	{
		const int projectId = 1;
		const int bugReportId = 1;
		const int linkToBugReportId = 1;

		var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId, UserId = "2", UserName = "Test User" });
		mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);
		AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor);

		mockBugReportRepo.Setup(br => br.DeleteBugReportLink(It.Is<int>(i => i == bugReportId), It.Is<int>(i => i == linkToBugReportId))).Verifiable();

		var result = await controller.DeleteLink(projectId, bugReportId, linkToBugReportId);
		Assert.NotNull(result);

		var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("ReportOverview", redirectToActionResult.ActionName);

		Assert.True(redirectToActionResult.RouteValues.ContainsKey("id"));
		var routeValueId = redirectToActionResult.RouteValues["id"];
		Assert.Equal(bugReportId, routeValueId);
	}

	[Fact]
	public async Task ReportOverview_ReturnsBadRequest_WhenBugReportIdLessThan1()
	{
		const int bugReportId = 0;

		var result = await controller.ReportOverview(bugReportId);

		Assert.IsType<BadRequestResult>(result);
	}

	[Fact]
	public async Task ReportOverview_ReturnsNotFound_WhenInvalidSessionProjectId()
	{
		const int projectId = 0;
		const int bugReportId = 1;

		var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId, UserId = "2", UserName = "Test User" });
		mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);

		var result = await controller.ReportOverview(bugReportId);

		Assert.IsType<NotFoundResult>(result);
	}

	[Fact]
	public async Task ReportOverview_RedirectsToHome_WhenNotAuthorized()
	{
		const int projectId = 1;
		const int bugReportId = 1;

		var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId });
		mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);
		AuthorizationHelper.AllowFailure(mockAuthorizationService, mockHttpContextAccessor);

		var result = await controller.ReportOverview(bugReportId);

		var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
		Assert.Equal("Index", redirectToActionResult.ActionName);
		Assert.Equal("Home", redirectToActionResult.ControllerName);
	}
}
