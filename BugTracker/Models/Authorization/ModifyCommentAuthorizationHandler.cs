using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Authorization
{
	public class ModifyCommentAuthorizationHandler : AuthorizationHandler<ModifyCommentRequirement, object>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ModifyCommentRequirement requirement, dynamic resource)
		{
			string userName = context.User.Identity.Name;
			if (userName == null)
			{
				return Task.CompletedTask;
			}

			string roleAdministrator = Enum.GetName(typeof(Roles), Roles.Administrator);
			string roleMember = Enum.GetName(typeof(Roles), Roles.Member);

			bool userIsSuperadministrator = AuthorizationHelper.UserIsSuperadministrator(userName);
			bool userIsProjectAdministrator = AuthorizationHelper.UserIsInProjectRole(userName, roleAdministrator, resource.ProjectId);
			bool userIsCommentAuthor = userName == resource.Author && AuthorizationHelper.UserIsInProjectRole(userName, roleMember, resource.ProjectId);

			if (userIsSuperadministrator || userIsProjectAdministrator || userIsCommentAuthor)
			{
				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}
	}

	public class ModifyCommentRequirement : IAuthorizationRequirement { }
}
