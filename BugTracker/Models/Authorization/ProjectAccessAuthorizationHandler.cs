using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Authorization
{
	public class ProjectAccessAuthorizationHandler : AuthorizationHandler<ProjectAccessRequirement, int>
	{
		private readonly string connectionString;

		public ProjectAccessAuthorizationHandler(string connectionString)
		{
			this.connectionString = connectionString;
		}

		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectAccessRequirement requirement, int projectId)
		{
			string userName = context.User.Identity.Name;
			if(userName == null)
			{
				return Task.CompletedTask;
			}

			bool userIsSuperadministrator = AuthorizationHelper.UserIsSuperadministrator(userName, connectionString);
			bool userIsProjectAdministrator = AuthorizationHelper.UserIsInProjectRole(userName, Enum.GetName(typeof(Roles), Roles.Administrator), projectId, connectionString);
			bool userIsProjectMember = AuthorizationHelper.UserIsInProjectRole(userName, Enum.GetName(typeof(Roles), Roles.Member), projectId, connectionString);
			
			if(userIsSuperadministrator || userIsProjectAdministrator || userIsProjectMember)
			{
				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}
	}

	public class ProjectAccessRequirement : IAuthorizationRequirement { }
}
