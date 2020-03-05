using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Authorization
{
	public class ProjectAdministratorAuthorizationHandler : AuthorizationHandler<ProjectAccessRequirement, int>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectAccessRequirement requirement, int projectId)
		{
			string userName = context.User.Identity.Name;
			if (userName == null)
			{
				return Task.CompletedTask;
			}

			bool userIsSuperadministrator = GetUserInRole(userName, Enum.GetName(typeof(Roles), Roles.Superadministrator), 0).Result;
			bool userIsProjectAdministrator = GetUserInRole(userName, Enum.GetName(typeof(Roles), Roles.Administrator), projectId).Result;

			if (userIsSuperadministrator || userIsProjectAdministrator)
			{
				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}

		// TODO -- move out to static helper class
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
}
