using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using BugTracker.Database.Context;

namespace BugTracker.Models.Authorization;

public class ProjectAccessAuthorizationHandler : AuthorizationHandler<ProjectAccessRequirement, int>
{
	private readonly ApplicationContext appContext;

	public ProjectAccessAuthorizationHandler(ApplicationContext appContext)
	{
		this.appContext = appContext;
	}

	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectAccessRequirement requirement, int projectId)
	{
		var userName = context.User.Identity?.Name;
		if(userName == null)
		{
			return Task.CompletedTask;
		}

		bool userIsSuperadministrator = AuthorizationHelper.UserIsSuperadministrator(userName, appContext);
		bool userIsProjectAdministrator = AuthorizationHelper.UserIsInProjectRole(userName, Enum.GetName(typeof(Roles), Roles.Administrator), projectId, appContext);
		bool userIsProjectMember = AuthorizationHelper.UserIsInProjectRole(userName, Enum.GetName(typeof(Roles), Roles.Member), projectId, appContext);
		
		if(userIsSuperadministrator || userIsProjectAdministrator || userIsProjectMember)
		{
			context.Succeed(requirement);
		}

		return Task.CompletedTask;
	}
}

public class ProjectAccessRequirement : IAuthorizationRequirement { }
