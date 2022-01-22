using BugTracker.Tests.Mocks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Tests.Helpers
{
	static class AuthorizationHelper
	{
		public static void AllowSuccess(Mock<IAuthorizationService> authorizationService, Mock<IHttpContextAccessor> mockHttpContextAccessor, int projectId = 1)
		{
			ConfigureHttpContext(mockHttpContextAccessor, projectId);
			Authorize(authorizationService, true);
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

		public static void Authorize(Mock<IAuthorizationService> authorizationService, bool willSucceed = true)
		{
			if (willSucceed)
			{
				authorizationService.Setup(_ => _.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(AuthorizationResult.Success);
				authorizationService.Setup(_ => _.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>())).ReturnsAsync(AuthorizationResult.Success);
			}
			else
			{
				authorizationService.Setup(_ => _.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(AuthorizationResult.Failed);
				authorizationService.Setup(_ => _.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>())).ReturnsAsync(AuthorizationResult.Failed);
			}
		}
	}
}
