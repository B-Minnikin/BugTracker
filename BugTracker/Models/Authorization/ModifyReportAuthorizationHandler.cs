using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using BugTracker.Database.Context;

namespace BugTracker.Models.Authorization;

public class ModifyReportAuthorizationHandler : AuthorizationHandler<ModifyReportRequirement, object>
{
	private readonly ApplicationContext appContext;

	public ModifyReportAuthorizationHandler(ApplicationContext appContext)
	{
		this.appContext = appContext;
	}

	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ModifyReportRequirement requirement, dynamic resource)
	{
		var userName = context.User.Identity?.Name;
		if(userName == null)
		{
			return Task.CompletedTask;
		}

		var roleAdministrator = Enum.GetName(typeof(Roles), Roles.Administrator);
		var roleMember = Enum.GetName(typeof(Roles), Roles.Member);

		var userIsSuperadministrator = AuthorizationHelper.UserIsSuperadministrator(userName, appContext);
		bool userIsProjectAdministrator = AuthorizationHelper.UserIsInProjectRole(userName, roleAdministrator, resource.ProjectId, appContext);
		bool userIsReportAuthor = userName == resource.PersonReporting && AuthorizationHelper.UserIsInProjectRole(userName, roleMember, resource.ProjectId, appContext);

		if(userIsSuperadministrator || userIsProjectAdministrator || userIsReportAuthor)
		{
			context.Succeed(requirement);
		}

		return Task.CompletedTask;
	}
}

public class ModifyReportRequirement : IAuthorizationRequirement { }
