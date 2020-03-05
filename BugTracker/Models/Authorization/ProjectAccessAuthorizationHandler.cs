using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Authorization
{
	public class ProjectAccessAuthorizationHandler : AuthorizationHandler<ProjectAccessRequirement, int>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectAccessRequirement requirement, int resource)
		{
			throw new NotImplementedException();
		}
	}

	public class ProjectAccessRequirement : IAuthorizationRequirement { }
}
