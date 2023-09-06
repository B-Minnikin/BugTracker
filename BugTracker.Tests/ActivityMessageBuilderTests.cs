using BugTracker.Extension_Methods;
using BugTracker.Models;
using BugTracker.Models.Authorization;
using BugTracker.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BugTracker.Database.Repository;
using BugTracker.Database.Repository.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace BugTracker.Tests
{
	public class ActivityMessageBuilderTests
	{
		private readonly ITestOutputHelper output;

		private readonly Mock<ILinkGenerator> mockLinkGenerator;

		private readonly Mock<IBugReportRepository> mockBugReportRepository;
		private readonly Mock<ApplicationUserManager> mockUserManager;
		private ActivityMessageBuilder builder;

		public ActivityMessageBuilderTests(ITestOutputHelper output)
		{
			this.output = output;

			mockLinkGenerator = new Mock<ILinkGenerator>();

			var mockProjectRepository = new Mock<IProjectRepository>();
			mockBugReportRepository = new Mock<IBugReportRepository>();
			var mockUserStore = new Mock<UserStore>("Fake connection string");
			mockUserManager = new Mock<ApplicationUserManager>(mockUserStore.Object, "dummy connection string");
			var mockMilestoneRepository = new Mock<IMilestoneRepository>();
			var mockBugReportStatesRepository = new Mock<IBugReportStatesRepository>();
			mockUserManager = new Mock<ApplicationUserManager>("dummy connection string");

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
			Assert.ThrowsAsync<ArgumentNullException>(() => builder.GenerateMessages(null));
		}

		[Fact]
		public void GetMessage_ThrowsArgumentNullException_IfActivityNull()
		{
			Assert.ThrowsAsync<ArgumentNullException>(() => builder.GetMessage(null));
		}

		[Fact]
		public async Task GetMessage_CreatesMessage_WhenValidActivityComment()
		{
			var activity = GetTestActivityComment();

			mockBugReportRepository.Setup(repo => repo.GetById(activity.BugReportId)).Returns(Task.FromResult(new BugReport { Title = "Comment Activity Test Report"}));

			// User
			const string userName = "John Smith";
			var user = new ApplicationUser
			{
				UserName = userName
			};
			mockUserManager.Setup(um => um.FindByIdAsync(activity.UserId).Result).Returns(user);

			// Link Generation
			var profileUri = "https://localhost:4000/Profile/View/" + activity.UserId;
			var bugReportUri = "https://localhost:4000/BugReport/ReportOverview/" + activity.BugReportId.ToString();
			mockLinkGenerator.Setup(lnk => lnk.GetPathByAction("View", "Profile", It.IsAny<object>())).Returns(profileUri);
			mockLinkGenerator.Setup(lnk => lnk.GetUriByAction("ReportOverview", "BugReport", It.IsAny<object>())).Returns(bugReportUri);

			// mock activity extensions
			var extensionMethodsDict = new Dictionary<string, int>();
			extensionMethodsDict.Add(nameof(ActivityBugReport.BugReportId), activity.BugReportId);
			ConfigureActivityMethodsMock(activity, extensionMethodsDict);
			var stringExtensionMethods = new Dictionary<string, string>();
			stringExtensionMethods.Add(nameof(Activity.UserId), activity.UserId);
			ConfigureActivityMethodsMock(activity, stringExtensionMethods);
			
			var expected = activity.Timestamp + " <a href=\"" + profileUri + "\">" + userName + 
				"</a> posted a new comment in bug report: <a href=\"" + bugReportUri + 
				"\">Comment Activity Test Report</a>.";

			var actual = await builder.GetMessage(activity);
			output.WriteLine("expected: {0}\nactual:     {1}", expected, actual);

			Assert.Equal(expected, actual);
		}

		private static void ConfigureActivityMethodsMock<T>(Activity activity, Dictionary<string, T> dict)
		{
			var activityMethodsMock = new Mock<IActivityMethods>();
			activityMethodsMock.Setup(ac => ac.HasProperty(activity, It.IsAny<string>())).Returns(true);

			foreach(var derivedProperty in dict)
			{
				activityMethodsMock.Setup(ac => ac.GetDerivedProperty<T>(activity, derivedProperty.Key)).Returns(derivedProperty.Value);
			}

			// inject mock extensions
			ActivityExtensions.Implementation = activityMethodsMock.Object;
		}

		private ActivityComment GetTestActivityComment()
		{
			// comment parameters
			var timestamp = new DateTime(2001, 01, 02, 12, 12, 12);
			const int projectId = 1;
			const ActivityMessage activityMessage = ActivityMessage.CommentPosted;
			const string userId = "1";
			const int bugReportId = 1;
			const int commentId = 1;

			return new ActivityComment(timestamp, projectId, activityMessage, userId, bugReportId, commentId);
		}
	}
}
