using System.Threading.Tasks;
using BugTracker.Database.Context;

namespace BugTracker.Models.Authorization;

public static class AuthorizationHelper
{
	public static bool UserIsSuperadministrator(string userName, ApplicationContext context)
	{
		return GetUserInRole(userName, "Superadministrator", -1, context).Result;
	}

	public static bool UserIsInProjectRole(string userName, string roleName, int id, ApplicationContext context)
	{
		return GetUserInRole(userName, roleName, id, context).Result;
	}

	public static async Task<bool> UserIsProfileOwner(string userName, string userId, ApplicationContext context)
	{
		var userManager = new ApplicationUserManager(context);
		var currentUser = await userManager.FindByNameAsync(userName);
		return currentUser.Id == userId;
	}

	private static async Task<bool> GetUserInRole(string userName, string roleName, int id, ApplicationContext context)
	{
		var userManager = new ApplicationUserManager(context);
		var user = await userManager.FindByNameAsync(userName);
		bool userIsInRole;

		if (roleName == "Superadministrator")
		{
			userIsInRole = await userManager.IsInRoleAsync(user, roleName);
		}
		else
		{
			userIsInRole = await userManager.IsInRoleAsync(user.Id, roleName, id);
		}

		return userIsInRole;
	}
}
