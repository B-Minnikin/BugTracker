using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Authorization
{
	public class ProjectAccessAuthorizationHandler : AuthorizationHandler<ProjectAccessRequirement, int>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectAccessRequirement requirement, int projectId)
		{
			string userName = context.User.Identity.Name;
			bool userIsSuperadministrator = GetUserInRole(userName, Enum.GetName(typeof(Roles), Roles.Superadministrator), 0).Result;
			bool userIsProjectAdministrator = GetUserInRole(userName, Enum.GetName(typeof(Roles), Roles.Administrator), projectId).Result;
			bool userIsProjectMember = GetUserInRole(userName, Enum.GetName(typeof(Roles), Roles.Member), projectId).Result;
			
			if(userIsSuperadministrator || userIsProjectAdministrator || userIsProjectMember)
			{
				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}

		protected async Task<bool> GetUserInRole(string userName, string roleName, int id)
		{
			ApplicationUserManager userManager = new ApplicationUserManager();
			var user = await userManager.FindByNameAsync(userName);
			bool userIsInRole;

			if (roleName == "Superadministrator")
			{
				userIsInRole = await userManager.IsInRoleAsync(user, roleName);
			}
			else
			{
				userIsInRole = await userManager.IsInRoleAsync(Int32.Parse(user.Id), roleName, id);
			}

			return userIsInRole;
		}
	}

	public class ProjectAccessRequirement : IAuthorizationRequirement { }
}
