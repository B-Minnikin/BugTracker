using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using BugTracker.Database.Context;

namespace BugTracker.Models.Authorization;

public class ProjectAdministratorAuthorizationHandler : AuthorizationHandler<ProjectAdministratorRequirement, int>
{
	private readonly ApplicationContext appContext;

	public ProjectAdministratorAuthorizationHandler(ApplicationContext appContext)
	{
		this.appContext = appContext;
	}

	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectAdministratorRequirement requirement, int projectId)
	{
		var userName = context.User.Identity?.Name;
		if (userName == null)
		{
			return Task.CompletedTask;
		}

		bool userIsSuperadministrator = AuthorizationHelper.UserIsSuperadministrator(userName, appContext);
		bool userIsProjectAdministrator = AuthorizationHelper.UserIsInProjectRole(userName, Enum.GetName(typeof(Roles), Roles.Administrator), projectId, appContext);

		if (userIsSuperadministrator || userIsProjectAdministrator)
		{
			context.Succeed(requirement);
		}

		return Task.CompletedTask;
	}
}

public class ProjectAdministratorRequirement : IAuthorizationRequirement { }
