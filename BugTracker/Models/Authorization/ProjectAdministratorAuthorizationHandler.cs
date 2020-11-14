using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Authorization
{
	public class ProjectAdministratorAuthorizationHandler : AuthorizationHandler<ProjectAdministratorRequirement, int>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectAdministratorRequirement requirement, int projectId)
		{
			string userName = context.User.Identity.Name;
			if (userName == null)
			{
				return Task.CompletedTask;
			}

			bool userIsSuperadministrator = AuthorizationHelper.UserIsSuperadministrator(userName);
			bool userIsProjectAdministrator = AuthorizationHelper.UserIsInProjectRole(userName, Enum.GetName(typeof(Roles), Roles.Administrator), projectId);

			if (userIsSuperadministrator || userIsProjectAdministrator)
			{
				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}
	}

	public class ProjectAdministratorRequirement : IAuthorizationRequirement { }
}
