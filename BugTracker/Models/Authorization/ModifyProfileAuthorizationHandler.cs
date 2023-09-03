using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using BugTracker.Database.Context;

namespace BugTracker.Models.Authorization;

public class ModifyProfileAuthorizationHandler : AuthorizationHandler<ModifyProfileRequirement, string>
{
	private readonly ApplicationContext appContext;

	public ModifyProfileAuthorizationHandler(ApplicationContext appContext)
	{
		this.appContext = appContext;
	}

	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ModifyProfileRequirement requirement, string userId)
	{
		var userName = context.User.Identity?.Name;
		if(userName == null)
		{
			return Task.CompletedTask;
		}
		
		var userIsSuperadministrator = AuthorizationHelper.UserIsSuperadministrator(userName, appContext);
		var userIsProfileOwner = AuthorizationHelper.UserIsProfileOwner(context.User.Identity.Name, userId, appContext).Result;

		if(userIsSuperadministrator || userIsProfileOwner)
		{
			context.Succeed(requirement);
		}

		return Task.CompletedTask;
	}
}

public class ModifyProfileRequirement : IAuthorizationRequirement { }
