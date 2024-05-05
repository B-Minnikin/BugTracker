using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace BugTracker.Models.Authorization;

public class ProjectAccessAuthorizationHandler : AuthorizationHandler<ProjectAccessRequirement, int>
{
	private readonly string connectionString;

	public ProjectAccessAuthorizationHandler(string connectionString)
	{
		this.connectionString = connectionString;
	}

	protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectAccessRequirement requirement, int projectId)
	{
		var userName = context.User.Identity?.Name;
		if(userName == null)
		{
			return;
		}

		var userIsSuperadministrator = await AuthorizationHelper.UserIsSuperadministrator(userName, connectionString);
		var userIsProjectAdministrator = await AuthorizationHelper.UserIsInProjectRole(userName, Enum.GetName(typeof(Roles), Roles.Administrator), projectId, connectionString);
		var userIsProjectMember = await AuthorizationHelper.UserIsInProjectRole(userName, Enum.GetName(typeof(Roles), Roles.Member), projectId, connectionString);
		
		if(userIsSuperadministrator || userIsProjectAdministrator || userIsProjectMember)
		{
			context.Succeed(requirement);
		}
	}
}

public class ProjectAccessRequirement : IAuthorizationRequirement { }
