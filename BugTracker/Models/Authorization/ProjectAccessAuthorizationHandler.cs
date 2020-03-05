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

		protected async Task<bool> GetUserInRole(string userName, string roleName, int id)
		{
			ApplicationUserManager userManager = new ApplicationUserManager();
			var user = await userManager.FindByNameAsync(userName);

			bool userIsInRole = await userManager.IsInRoleAsync(Int32.Parse(user.Id), roleName, id);

			return userIsInRole;
		}
	}

	public class ProjectAccessRequirement : IAuthorizationRequirement { }
}
