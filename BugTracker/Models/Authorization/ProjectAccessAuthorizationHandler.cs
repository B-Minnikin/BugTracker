using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace BugTracker.Models.Authorization;

public class ProjectAccessAuthorizationHandler : AuthorizationHandler<ProjectAccessRequirement, int>
{
	private readonly ApplicationUserManager userManager;

	public ProjectAccessAuthorizationHandler(ApplicationUserManager userManager)
	{
		this.userManager = userManager;
	}

	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectAccessRequirement requirement, int projectId)
	{
		var userName = context.User.Identity?.Name;
		if(userName == null)
		{
			return Task.CompletedTask;
		}

		var userIsSuperadministrator = AuthorizationHelper.UserIsSuperadministrator(userName, userManager);
		var userIsProjectAdministrator = AuthorizationHelper.UserIsInProjectRole(userName, Enum.GetName(typeof(ProjectRoles), ProjectRoles.Administrator), projectId, userManager);
		var userIsProjectMember = AuthorizationHelper.UserIsInProjectRole(userName, Enum.GetName(typeof(ProjectRoles), ProjectRoles.Member), projectId, userManager);
		
		if(userIsSuperadministrator || userIsProjectAdministrator || userIsProjectMember)
		{
			context.Succeed(requirement);
		}

		return Task.CompletedTask;
	}
}

public class ProjectAccessRequirement : IAuthorizationRequirement { }
