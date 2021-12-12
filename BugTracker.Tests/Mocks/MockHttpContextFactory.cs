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
			return GetHttpContext(1, "Test User");
		}

		public static HttpContext GetHttpContext(int projectId)
		{
			return GetHttpContext(projectId, "Test User");
		}

		public static HttpContext GetHttpContext(int projectId, string userName)
		{
			MockHttpSession mockSession = new MockHttpSession();
			mockSession.SetInt32("currentProject", projectId);

			var identity = new GenericIdentity(userName);
			var contextUser = new Mock<ClaimsPrincipal>(identity);
			var httpContext = new DefaultHttpContext()
			{
				User = contextUser.Object,
				Session = mockSession
			};

			return httpContext;
		}
	}
}
