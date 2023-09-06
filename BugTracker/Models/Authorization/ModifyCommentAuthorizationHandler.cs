using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace BugTracker.Models.Authorization;

public class ModifyCommentAuthorizationHandler : AuthorizationHandler<ModifyCommentRequirement, object>
{
	private readonly ApplicationUserManager userManager;

	public ModifyCommentAuthorizationHandler(ApplicationUserManager userManager)
	{
		this.userManager = userManager;
	}

	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ModifyCommentRequirement requirement, dynamic resource)
	{
		var userName = context.User.Identity?.Name;
		if (userName == null)
		{
			return Task.CompletedTask;
		}

		var roleAdministrator = Enum.GetName(typeof(ProjectRoles), ProjectRoles.Administrator);
		var roleMember = Enum.GetName(typeof(ProjectRoles), ProjectRoles.Member);

		var userIsSuperadministrator = AuthorizationHelper.UserIsSuperadministrator(userName, userManager);
		bool userIsProjectAdministrator = AuthorizationHelper.UserIsInProjectRole(userName, roleAdministrator, resource.ProjectId, userManager);
		bool userIsCommentAuthor = userName == resource.Author && AuthorizationHelper.UserIsInProjectRole(userName, roleMember, resource.ProjectId, userManager);

		if (userIsSuperadministrator || userIsProjectAdministrator || userIsCommentAuthor)
		{
			context.Succeed(requirement);
		}

		return Task.CompletedTask;
	}
}

public class ModifyCommentRequirement : IAuthorizationRequirement { }
