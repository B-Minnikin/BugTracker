using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace BugTracker.Models.Authorization;
	
public class ProjectAdministratorAuthorizationHandler : AuthorizationHandler<ProjectAdministratorRequirement, int>
{
	private readonly string connectionString;

	public ProjectAdministratorAuthorizationHandler(string connectionString)
	{
		this.connectionString = connectionString;
	}

	protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectAdministratorRequirement requirement, int projectId)
	{
		var userName = context.User.Identity?.Name;
		if (userName == null)
		{
			return;
		}

		var userIsSuperadministrator = await AuthorizationHelper.UserIsSuperadministrator(userName, connectionString);
		var userIsProjectAdministrator = await AuthorizationHelper.UserIsInProjectRole(userName, Enum.GetName(typeof(Roles), Roles.Administrator), projectId, connectionString);

		if (userIsSuperadministrator || userIsProjectAdministrator)
		{
			context.Succeed(requirement);
		}
	}
}

public class ProjectAdministratorRequirement : IAuthorizationRequirement { }
