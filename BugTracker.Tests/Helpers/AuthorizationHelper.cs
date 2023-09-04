using BugTracker.Tests.Mocks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace BugTracker.Tests.Helpers
{
	static class AuthorizationHelper
	{
		public static void AllowSuccess(Mock<IAuthorizationService> authorizationService, Mock<IHttpContextAccessor> mockHttpContextAccessor, int projectId = 1)
		{
			ConfigureHttpContext(mockHttpContextAccessor, projectId);
			Authorize(authorizationService);
		}

		public static void AllowFailure(Mock<IAuthorizationService> authorizationService, Mock<IHttpContextAccessor> mockHttpContextAccessor, int projectId = 1)
		{
			ConfigureHttpContext(mockHttpContextAccessor, projectId);
			Authorize(authorizationService, false);
		}

		private static void ConfigureHttpContext(Mock<IHttpContextAccessor> mockHttpContextAccessor, int projectId)
		{
			var httpContext = MockHttpContextFactory.GetHttpContext(new HttpContextFactoryOptions { ProjectId = projectId});
			mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(httpContext);
		}

		private static void Authorize(Mock<IAuthorizationService> authorizationService, bool willSucceed = true)
		{
			var result = willSucceed ? AuthorizationResult.Success() : AuthorizationResult.Failed();
			
			authorizationService.Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(result);
			authorizationService.Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>())).ReturnsAsync(result);
		}
	}
}
