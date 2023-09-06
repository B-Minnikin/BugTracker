using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace BugTracker.Models.Authorization;

public class ModifyProfileAuthorizationHandler : AuthorizationHandler<ModifyProfileRequirement, string>
{
	private readonly ApplicationUserManager userManager;

	public ModifyProfileAuthorizationHandler(ApplicationUserManager userManager)
	{
		this.userManager = userManager;
	}

	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ModifyProfileRequirement requirement, string userId)
	{
		var userName = context.User.Identity?.Name;
		if(userName == null)
		{
			return Task.CompletedTask;
		}
		
		var userIsSuperadministrator = AuthorizationHelper.UserIsSuperadministrator(userName, userManager);
		var userIsProfileOwner = AuthorizationHelper.UserIsProfileOwner(context.User.Identity.Name, userId, userManager).Result;

		if(userIsSuperadministrator || userIsProfileOwner)
		{
			context.Succeed(requirement);
		}

		return Task.CompletedTask;
	}
}

public class ModifyProfileRequirement : IAuthorizationRequirement { }
