using BugTracker.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using System.Security.Principal;

namespace BugTracker.Tests.Mocks
{
	public class MockHttpContextFactory
	{
		public static HttpContext GetHttpContext()
		{
			return GetHttpContext(new HttpContextFactoryOptions());
		}

		public static HttpContext GetHttpContext(HttpContextFactoryOptions options)
		{
			MockHttpSession mockSession = new MockHttpSession();
			mockSession.SetInt32("currentProject", options.ProjectId);

			var identity = new GenericIdentity(options.UserName);
			var contextUser = new Mock<ClaimsPrincipal>(identity);
			contextUser.Setup(_ => _.Identity.Name).Returns(options.UserName);
			contextUser.Setup(_ => _.FindFirst(It.IsAny<string>())).Returns(new Claim(ClaimTypes.NameIdentifier, options.UserId));
			var httpContext = new DefaultHttpContext()
			{
				User = contextUser.Object,
				Session = mockSession
			};

			return httpContext;
		}
	}
}
