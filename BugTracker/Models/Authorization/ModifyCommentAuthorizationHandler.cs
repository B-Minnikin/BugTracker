using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using BugTracker.Database.Context;

namespace BugTracker.Models.Authorization;

public class ModifyCommentAuthorizationHandler : AuthorizationHandler<ModifyCommentRequirement, object>
{
	private readonly ApplicationContext appContext;

	public ModifyCommentAuthorizationHandler(ApplicationContext appContext)
	{
		this.appContext = appContext;
	}

	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ModifyCommentRequirement requirement, dynamic resource)
	{
		var userName = context.User.Identity?.Name;
		if (userName == null)
		{
			return Task.CompletedTask;
		}

		var roleAdministrator = Enum.GetName(typeof(Roles), Roles.Administrator);
		var roleMember = Enum.GetName(typeof(Roles), Roles.Member);

		var userIsSuperadministrator = AuthorizationHelper.UserIsSuperadministrator(userName, appContext);
		bool userIsProjectAdministrator = AuthorizationHelper.UserIsInProjectRole(userName, roleAdministrator, resource.ProjectId, appContext);
		bool userIsCommentAuthor = userName == resource.Author && AuthorizationHelper.UserIsInProjectRole(userName, roleMember, resource.ProjectId, appContext);

		if (userIsSuperadministrator || userIsProjectAdministrator || userIsCommentAuthor)
		{
			context.Succeed(requirement);
		}

		return Task.CompletedTask;
	}
}

public class ModifyCommentRequirement : IAuthorizationRequirement { }
