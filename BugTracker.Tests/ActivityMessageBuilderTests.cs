using BugTracker.Extension_Methods;
using BugTracker.Models;
using BugTracker.Models.Authorization;
using BugTracker.Repository;
using BugTracker.Repository.Interfaces;
using BugTracker.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace BugTracker.Tests
{
	public class ActivityMessageBuilderTests
	{
		private readonly Mock<IHttpContextAccessor> mockAccessor;
		private readonly Mock<ILinkGenerator> mockLinkGenerator;

		private readonly Mock<IProjectRepository> mockProjectRepository;
		private readonly Mock<IBugReportRepository> mockBugReportRepository;
		private readonly Mock<IMilestoneRepository> mockMilestoneRepository;
		private readonly Mock<IBugReportStatesRepository> mockBugReportStatesRepository;
		private readonly Mock<ApplicationUserManager> mockUserManager;
		private ActivityMessageBuilder builder;

		public ActivityMessageBuilderTests()
		{
			mockAccessor = new Mock<IHttpContextAccessor>();
			mockLinkGenerator = new Mock<ILinkGenerator>();
			//mockLinkGenerator.Setup(lnk => lnk.GetPathByAction("", "", new { })).Returns("");

			mockProjectRepository = new Mock<IProjectRepository>();
			mockBugReportRepository = new Mock<IBugReportRepository>();
			mockMilestoneRepository = new Mock<IMilestoneRepository>();
			mockBugReportStatesRepository = new Mock<IBugReportStatesRepository>();
			mockUserManager = new Mock<ApplicationUserManager>();

			// inject mocked dependencies
			builder = new ActivityMessageBuilder(
					mockLinkGenerator.Object,
					mockUserManager.Object, mockProjectRepository.Object,
					mockBugReportRepository.Object,
					mockMilestoneRepository.Object,
					mockBugReportStatesRepository.Object
				);
		}

		[Fact]
		public void GenerateMessages_ThrowsArgumentNullException_IfActivitiesNull()
		{
			Assert.Throws<ArgumentNullException>(() => builder.GenerateMessages(null));
		}

		[Fact]
		public void GetMessage_ThrowsArgumentNullException_IfActivityNull()
		{
			Assert.Throws<ArgumentNullException>(() => builder.GetMessage(null));
		}

		[Fact]
		public void GetMessage_CreatesMessage_WhenValidActivityComment()
		{
			ActivityComment activity = GetTestActivityComment();

			mockBugReportRepository.Setup(repo => repo.GetById(activity.BugReportId)).Returns(new BugReport { Title = "Comment Activity Test Report"});

			// User
			string userName = "John Smith";
			var user = new IdentityUser();
			user.UserName = userName;
			mockUserManager.Setup(um => um.FindByIdAsync(activity.UserId.ToString()).Result).Returns(user);

			// Link Generation
			string profileURI = "https://localhost:4000/Profile/View/" + activity.UserId.ToString();
			string bugReportURI = "https://localhost:4000/BugReport/ReportOverview/" + activity.BugReportId.ToString();
			mockLinkGenerator.Setup(lnk => lnk.GetPathByAction("View", "Profile", It.IsAny<object>())).Returns(profileURI);
			mockLinkGenerator.Setup(lnk => lnk.GetUriByAction("ReportOverview", "BugReport", It.IsAny<object>())).Returns(bugReportURI);

			// mock activity extensions
			var extensionMethodsDict = new Dictionary<string, int>();
			extensionMethodsDict.Add(nameof(ActivityBugReport.BugReportId), activity.BugReportId);
			extensionMethodsDict.Add(nameof(Activity.UserId), activity.UserId);
			var activityMethodsMock = ConfigureActivityMethodsMock(activity, extensionMethodsDict);
			
			string expected = activity.Timestamp + " <a href=\"" + profileURI + "\">" + userName + 
				"</a> posted a new comment in bug report: <a href=\"" + bugReportURI + 
				"\">Comment Activity Test Report</a>.";

			// act
			var actual = builder.GetMessage(activity);

			// assert
			Assert.Equal(expected, actual);
		}

		private Mock<IActivityMethods> ConfigureActivityMethodsMock(Activity activity, Dictionary<string, int> dict)
		{
			var activityMethodsMock = new Mock<IActivityMethods>();
			activityMethodsMock.Setup(ac => ac.HasProperty(activity, It.IsAny<string>())).Returns(true);

			foreach(var derivedProperty in dict)
			{
				activityMethodsMock.Setup(ac => ac.GetDerivedProperty<int>(activity, derivedProperty.Key)).Returns(derivedProperty.Value);
			}

			// inject mock extensions
			ActivityExtensions.Implementation = activityMethodsMock.Object;

			return activityMethodsMock;
		}

		private ActivityComment GetTestActivityComment()
		{
			// comment parameters
			DateTime timestamp = new DateTime(2001, 01, 02, 12, 12, 12);
			int projectId = 1;
			ActivityMessage activityMessage = ActivityMessage.CommentPosted;
			int userId = 1;
			int bugReportId = 1;
			int bugReportCommentId = 1;

			return new ActivityComment(timestamp, projectId, activityMessage, userId, bugReportId, bugReportCommentId);
		}
	}
}
