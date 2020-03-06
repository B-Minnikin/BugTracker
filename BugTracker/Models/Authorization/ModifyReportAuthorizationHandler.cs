using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Authorization
{
	public class ModifyReportAuthorizationHandler : AuthorizationHandler<ModifyReportRequirement, object>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ModifyReportRequirement requirement, dynamic resource)
		{
			string userName = context.User.Identity.Name;
			if(userName == null)
			{
				return Task.CompletedTask;
			}

			string roleAdministrator = Enum.GetName(typeof(Roles), Roles.Administrator);
			string roleMember = Enum.GetName(typeof(Roles), Roles.Member);

			bool userIsSuperadministrator = AuthorizationHelper.UserIsSuperadministrator(userName);
			bool userIsProjectAdministrator = AuthorizationHelper.UserIsInProjectRole(userName, roleAdministrator, resource.ProjectId);
			bool userIsReportAuthor = userName == resource.PersonReporting && AuthorizationHelper.UserIsInProjectRole(userName, roleMember, resource.ProjectId);

			if(userIsSuperadministrator || userIsProjectAdministrator || userIsReportAuthor)
			{
				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}
	}

	public class ModifyReportRequirement : IAuthorizationRequirement { }
}
