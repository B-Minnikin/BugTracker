using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BugTracker.Services
{
	public class ApplicationLinkGenerator : ILinkGenerator
	{
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly LinkGenerator linkGenerator;

		public ApplicationLinkGenerator(IHttpContextAccessor httpContextAccessor,
											LinkGenerator linkGenerator)
		{
			this.httpContextAccessor = httpContextAccessor;
			this.linkGenerator = linkGenerator;
		}

		public string GetPathByAction(string action, string controller, object values)
		{
			if (httpContextAccessor?.HttpContext is null) throw new ApplicationException("HttpContext is null");
			
			return linkGenerator.GetPathByAction(httpContextAccessor.HttpContext, action, controller, values);
		}

		public string GetUriByAction(string action, string controller, object values)
		{
			if (httpContextAccessor?.HttpContext is null) throw new ApplicationException("HttpContext is null");
			
			return linkGenerator.GetUriByAction(httpContextAccessor.HttpContext, action, controller, values);
		}
	}
}
