using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace BugTracker.Models.Authorization;

public class ModifyProfileAuthorizationHandler : AuthorizationHandler<ModifyProfileRequirement, int>
{
	private readonly string connectionString;

	public ModifyProfileAuthorizationHandler(string connectionString)
	{
		this.connectionString = connectionString;
	}

	protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ModifyProfileRequirement requirement, int userId)
	{
		var userName = context.User.Identity?.Name;
		if(userName == null)
		{
			return;
		}
		
		var userIsSuperadministrator = await AuthorizationHelper.UserIsSuperadministrator(userName, connectionString);
		var userIsProfileOwner = await AuthorizationHelper.UserIsProfileOwner(context.User.Identity.Name, userId, connectionString);

		if(userIsSuperadministrator || userIsProfileOwner)
		{
			context.Succeed(requirement);
		}
	}
}

public class ModifyProfileRequirement : IAuthorizationRequirement { }
