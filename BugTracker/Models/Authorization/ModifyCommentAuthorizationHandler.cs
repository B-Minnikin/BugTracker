using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace BugTracker.Models.Authorization;

public class ModifyCommentAuthorizationHandler : AuthorizationHandler<ModifyCommentRequirement, object>
{
	private readonly string connectionString;

	public ModifyCommentAuthorizationHandler(string connectionString)
	{
		this.connectionString = connectionString;
	}

	protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ModifyCommentRequirement requirement, dynamic resource)
	{
		var userName = context.User.Identity?.Name;
		if (userName == null)
		{
			return;
		}

		var roleAdministrator = Enum.GetName(typeof(Roles), Roles.Administrator);
		var roleMember = Enum.GetName(typeof(Roles), Roles.Member);

		var userIsSuperadministrator = await AuthorizationHelper.UserIsSuperadministrator(userName, connectionString);
		var userIsProjectAdministrator = await AuthorizationHelper.UserIsInProjectRole(userName, roleAdministrator, resource.ProjectId, connectionString);
		var userIsCommentAuthor = userName == resource.Author && await AuthorizationHelper.UserIsInProjectRole(userName, roleMember, resource.ProjectId, connectionString);

		if (userIsSuperadministrator || userIsProjectAdministrator || userIsCommentAuthor)
		{
			context.Succeed(requirement);
		}
	}
}

public class ModifyCommentRequirement : IAuthorizationRequirement { }
