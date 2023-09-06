using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace BugTracker.Models.Authorization;

public class ModifyReportAuthorizationHandler : AuthorizationHandler<ModifyReportRequirement, object>
{
	private readonly ApplicationUserManager userManager;

	public ModifyReportAuthorizationHandler(ApplicationUserManager userManager)
	{
		this.userManager = userManager;
	}

	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ModifyReportRequirement requirement, dynamic resource)
	{
		var userName = context.User.Identity?.Name;
		if(userName == null)
		{
			return Task.CompletedTask;
		}

		var roleAdministrator = Enum.GetName(typeof(ProjectRoles), ProjectRoles.Administrator);
		var roleMember = Enum.GetName(typeof(ProjectRoles), ProjectRoles.Member);

		var userIsSuperadministrator = AuthorizationHelper.UserIsSuperadministrator(userName, userManager);
		bool userIsProjectAdministrator = AuthorizationHelper.UserIsInProjectRole(userName, roleAdministrator, resource.ProjectId, userManager);
		bool userIsReportAuthor = userName == resource.PersonReporting && AuthorizationHelper.UserIsInProjectRole(userName, roleMember, resource.ProjectId, userManager);

		if(userIsSuperadministrator || userIsProjectAdministrator || userIsReportAuthor)
		{
			context.Succeed(requirement);
		}

		return Task.CompletedTask;
	}
}

public class ModifyReportRequirement : IAuthorizationRequirement { }
