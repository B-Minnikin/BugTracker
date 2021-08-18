using BugTracker.Models;
using BugTracker.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace BugTracker.Tests
{
	public class ActivityMessageBuilderTests
	{
		private readonly Moq.Mock<IHttpContextAccessor> mockAccessor;
		private readonly Mock<IProjectRepository> mockProjectRepository;
		private readonly Mock<LinkGenerator> linkGenerator;
		private ActivityMessageBuilder builder;

		public ActivityMessageBuilderTests()
		{
			mockAccessor = new Mock<IHttpContextAccessor>();
			mockProjectRepository = new Mock<IProjectRepository>();
			linkGenerator = new Mock<LinkGenerator>();

			// inject mocked dependencies
			builder = new ActivityMessageBuilder(
					mockAccessor.Object, linkGenerator.Object, mockProjectRepository.Object
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
	}
}
