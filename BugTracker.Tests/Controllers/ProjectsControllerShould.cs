﻿using BugTracker.Controllers;
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
using System;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Identity;
using BugTracker.Repository;
using System.Security.Claims;

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
		private readonly Mock<BugTracker.Repository.UserStore> mockUserStore;
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
			mockUserStore = new Mock<UserStore>("Fake connection string");
			mockUserManager = new Mock<ApplicationUserManager>(mockUserStore.Object, "Fake connection string");
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
		public async Task Overview_ReturnsNotFound_IfIdInvalid()
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

		[Fact]
		public async Task CreateProject_RedirectsToOverview_IfModelValid()
		{
			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor);

			Project model = new Project
			{
				Name = "Test name",
				Description = "Test description",
				CreationTime = DateTime.Now,
				LastUpdateTime = DateTime.Now,
				Hidden = false,
				BugReports = new List<BugReport>()
			};
			ActivityProject activityProject = new();

			mockUserManager.Setup(_ => _.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new IdentityUser()));
			mockProjectRepo.Setup(_ => _.Add(It.IsAny<Project>())).Returns(Task.FromResult(model));
			mockActivityRepo.Setup(_ => _.Add(It.IsAny<ActivityProject>())).Returns(Task.FromResult((Activity)activityProject));

			// Get connection string
			Mock<IConfigurationSection> mockConfigurationSection = new();
			mockConfigurationSection.SetupGet(x => x[It.Is<string>(s => s == "DBConnectionString")]).Returns("Mock Connection String");
			mockConfiguration.Setup(x => x.GetSection(It.Is<string>(k => k == "ConnectionStrings"))).Returns(mockConfigurationSection.Object);

			// Role results within UserManager
			mockUserStore.Setup(x => x.IsInRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<System.Threading.CancellationToken>())).Returns(Task.FromResult(true));
			mockUserStore.Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<System.Threading.CancellationToken>())).Returns(Task.FromResult(IdentityResult.Success));
			mockUserManager.Setup(x => x.RegisterUserStore(mockUserStore.Object));
			
			var result = await controller.CreateProject(model);

			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Overview", redirectToActionResult.ActionName);
		}

		[Fact]
		public async Task CreateProject_ReturnsBadRequest_IfModelInvalid()
		{
			Project model = new Project
			{
				Name = "Test name",
				Description = "Test description",
				CreationTime = DateTime.Now,
				LastUpdateTime = DateTime.Now,
				Hidden = false,
				BugReports = new List<BugReport>()
			};
			ActivityProject activityProject = new ActivityProject();

			mockProjectRepo.Setup(_ => _.Add(It.IsAny<Project>())).Returns(Task.FromResult(model));
			mockActivityRepo.Setup(_ => _.Add(It.IsAny<ActivityProject>())).Returns(Task.FromResult((Activity)activityProject));
			controller.ModelState.AddModelError("Test key", "Error message");

			var result = await controller.CreateProject(model);

			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.IsType<SerializableError>(badRequestResult.Value);
		}

		[Fact]
		public void DeleteProject_ReturnsBadRequest_IfIdLessThan1()
		{
			var projectId = 0;
			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor);

			var result = controller.DeleteProject(projectId);

			var badRequestResult = Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		public void DeleteProject_RedirectsToProjects_IfNotAuthorized()
		{
			int projectId = 1;
			AuthorizationHelper.AllowFailure(mockAuthorizationService, mockHttpContextAccessor);

			mockProjectRepo.Setup(_ => _.Delete(It.IsAny<int>()));

			var result = controller.DeleteProject(projectId);

			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Projects", redirectToActionResult.ActionName);
		}

		[Fact]
		public async Task Edit_Get_RedirectsToOverview_IfNotAuthorized()
		{
			int projectId = 1;
			AuthorizationHelper.AllowFailure(mockAuthorizationService, mockHttpContextAccessor);

			var result = await controller.Edit(projectId);

			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Overview", redirectToActionResult.ActionName);
			Assert.True(redirectToActionResult.RouteValues.ContainsKey("projectId"));
		}

		[Fact]
		public async Task Edit_Get_ReturnsBadRequest_IfIdLessThan1()
		{
			var projectId = 0;
			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor);

			var result = await controller.Edit(projectId);

			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		public async Task Edit_Get_ReturnsProjectViewModel_IfValidId()
		{
			int projectId = 1;
			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor);

			var testProject = new Project { ProjectId = 1, Name = "Test Project"};
			mockProjectRepo.Setup(_ => _.GetById(It.IsAny<int>())).Returns(Task.FromResult(testProject));

			var result = await controller.Edit(projectId);

			var viewResult = Assert.IsType<ViewResult>(result);
			Assert.IsAssignableFrom<EditProjectViewModel>(viewResult.ViewData.Model);
		}

		[Fact]
		public async Task Edit_Post_ReturnsView_IfModelNotValid()
		{
			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor);

			var dummyProject = new Project { Name = "Test Project", Description = "Test project" };
			var viewModel = new EditProjectViewModel
			{
				Project = dummyProject
			};
			controller.ModelState.AddModelError("Test key", "Error message");

			mockProjectRepo.Setup(_ => _.Update(It.IsAny<Project>())).Returns(Task.FromResult(dummyProject));

			var result = await controller.Edit(viewModel);

			var viewResult = Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public async Task Edit_Post_ReturnsView_IfNotAuthorized()
		{
			AuthorizationHelper.AllowFailure(mockAuthorizationService, mockHttpContextAccessor);

			var dummyProject = new Project { ProjectId = 1, Name = "Test Project", Description = "Test project" };
			var viewModel = new EditProjectViewModel
			{
				Project = dummyProject
			};

			var result = await controller.Edit(viewModel);

			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public async Task Edit_Post_RedirectsToOverview_IfModelValid()
		{
			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor);

			var dummyProject = new Project 
			{
				ProjectId = 1,
				Name = "Test Project",
				Description = "Test project" ,
				Hidden = false,
				LastUpdateTime = DateTime.Now
			};
			var viewModel = new EditProjectViewModel
			{
				Project = dummyProject
			};

			mockProjectRepo.Setup(x => x.GetById(It.IsAny<int>())).Returns(Task.FromResult(dummyProject));
			mockProjectRepo.Setup(_ => _.Update(It.IsAny<Project>())).Returns(Task.FromResult(dummyProject));
			mockActivityRepo.Setup(_ => _.Add(It.IsAny<Activity>())).Returns(Task.FromResult(new ActivityProject() as Activity));

			var result = await controller.Edit(viewModel);

			var redirectResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Overview", redirectResult.ActionName);
			var routeValueId = redirectResult.RouteValues["projectId"];
			Assert.Equal(dummyProject.ProjectId, routeValueId);
		}

		[Fact]
		public async Task Invites_Get_ReturnsBadRequest_IfIdLessThan1()
		{
			var projectId = 0;
			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor);

			var result = await controller.Invites(projectId);

			Assert.IsType<BadRequestResult>(result);
		}

		[Fact]
		public async Task Invites_Get_RedirectsToOverview_IfNotAuthorized()
		{
			int projectId = 1;
			AuthorizationHelper.AllowFailure(mockAuthorizationService, mockHttpContextAccessor);

			var result = await controller.Invites(projectId);

			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Overview", redirectToActionResult.ActionName);
			Assert.True(redirectToActionResult.RouteValues.ContainsKey("projectId"));
		}

		[Fact]
		public async Task Invites_Get_ReturnsViewModel_WhenIdValid()
		{
			int projectId = 1;
			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor);

			var testProject = new Project { ProjectId = 1, Name = "Test Project" };
			mockProjectRepo.Setup(_ => _.GetById(It.IsAny<int>())).Returns(Task.FromResult(testProject));

			var result = await controller.Invites(projectId);

			var viewResult = Assert.IsType<ViewResult>(result);
			Assert.IsAssignableFrom<InvitesViewModel>(viewResult.ViewData.Model);
		}

		[Fact]
		public async Task Invites_Post_RedirectsToOverview_IfNotAuthorized()
		{
			InvitesViewModel invitesViewModel = new InvitesViewModel
			{
				EmailAddress = "test@email.com",
				ProjectId = 1
			};
			AuthorizationHelper.AllowFailure(mockAuthorizationService, mockHttpContextAccessor);

			var result = await controller.Invites(invitesViewModel);

			var viewResult = Assert.IsType<ViewResult>(result);
			Assert.IsAssignableFrom<InvitesViewModel>(viewResult.ViewData.Model);
		}

		[Fact]
		public async Task Invites_Post_ReturnsView_IfModelNotValid()
		{
			var invitesViewModel = new InvitesViewModel
			{
				ProjectId = 1
			};
			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor);

			controller.ModelState.AddModelError("Test key", "Error message");

			var result = await controller.Invites(invitesViewModel);

			var viewResult = Assert.IsType<ViewResult>(result);
			Assert.IsAssignableFrom<InvitesViewModel>(viewResult.ViewData.Model);
		}

		[Fact]
		public async Task Invites_Post_RedirectsToOverview_IfModelValid()
		{
			AuthorizationHelper.AllowSuccess(mockAuthorizationService, mockHttpContextAccessor);

			var invitesViewModel = new InvitesViewModel
			{
				ProjectId = 1,
				EmailAddress = "Test@email.com"
			};

			var project = new Project
			{
				ProjectId = 1,
				Name = "Test Name",
				Description = "Test description",
				CreationTime = DateTime.Now,
				LastUpdateTime = DateTime.Now,
				Hidden = false
			};
			
			mockProjectRepo.Setup(x => x.GetById(It.IsAny<int>())).Returns(Task.FromResult(project));
			mockUserManager.Setup(_ => _.GetUserAsync(It.IsAny<ClaimsPrincipal>())).Returns(Task.FromResult(new IdentityUser()));
			mockProjectInviter.Setup(_ => _.AddProjectInvitation(It.IsAny<ProjectInvitation>()));

			var result = await controller.Invites(invitesViewModel);

			var redirectResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Overview", redirectResult.ActionName);
			var routeValueId = redirectResult.RouteValues["projectId"];
			Assert.Equal(invitesViewModel.ProjectId, routeValueId);
		}
	}
}
