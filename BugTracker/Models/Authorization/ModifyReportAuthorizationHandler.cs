using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace BugTracker.Models.Authorization;

public class ModifyReportAuthorizationHandler : AuthorizationHandler<ModifyReportRequirement, object>
{
	private readonly string connectionString;

	public ModifyReportAuthorizationHandler(string connectionString)
	{
		this.connectionString = connectionString;
	}

	protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ModifyReportRequirement requirement, dynamic resource)
	{
		var userName = context.User.Identity?.Name;
		if(userName == null)
		{
			return;
		}

		var roleAdministrator = Enum.GetName(typeof(Roles), Roles.Administrator);
		var roleMember = Enum.GetName(typeof(Roles), Roles.Member);

		var userIsSuperadministrator = await AuthorizationHelper.UserIsSuperadministrator(userName, connectionString);
		bool userIsProjectAdministrator = await AuthorizationHelper.UserIsInProjectRole(userName, roleAdministrator, resource.ProjectId, connectionString);
		bool userIsReportAuthor = userName == resource.PersonReporting && await AuthorizationHelper.UserIsInProjectRole(userName, roleMember, resource.ProjectId, connectionString);

		if(userIsSuperadministrator || userIsProjectAdministrator || userIsReportAuthor)
		{
			context.Succeed(requirement);
		}
	}
}

public class ModifyReportRequirement : IAuthorizationRequirement { }
