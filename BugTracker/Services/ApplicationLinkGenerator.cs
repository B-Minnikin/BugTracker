using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
			return linkGenerator.GetPathByAction(httpContextAccessor.HttpContext, action, controller, values);
		}

		public string GetUriByAction(string action, string controller, object values)
		{
			return linkGenerator.GetUriByAction(httpContextAccessor.HttpContext, action, controller, values);
		}
	}
}
