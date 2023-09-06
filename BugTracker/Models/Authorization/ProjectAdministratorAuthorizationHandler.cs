using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace BugTracker.Models.Authorization;

public class ProjectAdministratorAuthorizationHandler : AuthorizationHandler<ProjectAdministratorRequirement, int>
{
	private readonly ApplicationUserManager userManager;

	public ProjectAdministratorAuthorizationHandler(ApplicationUserManager userManager)
	{
		this.userManager = userManager;
	}

	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectAdministratorRequirement requirement, int projectId)
	{
		var userName = context.User.Identity?.Name;
		if (userName == null)
		{
			return Task.CompletedTask;
		}

		var userIsSuperadministrator = AuthorizationHelper.UserIsSuperadministrator(userName, userManager);
		var userIsProjectAdministrator = AuthorizationHelper.UserIsInProjectRole(userName, Enum.GetName(typeof(ProjectRoles), ProjectRoles.Administrator), projectId, userManager);

		if (userIsSuperadministrator || userIsProjectAdministrator)
		{
			context.Succeed(requirement);
		}

		return Task.CompletedTask;
	}
}

public class ProjectAdministratorRequirement : IAuthorizationRequirement { }
