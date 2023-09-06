using System.Threading.Tasks;

namespace BugTracker.Models.Authorization;

public static class AuthorizationHelper
{
	public static bool UserIsSuperadministrator(string userName, ApplicationUserManager userManager)
	{
		return GetUserInRole(userName, "Superadministrator", -1, userManager).Result;
	}

	public static bool UserIsInProjectRole(string userName, string roleName, int id, ApplicationUserManager userManager)
	{
		return GetUserInRole(userName, roleName, id, userManager).Result;
	}

	public static async Task<bool> UserIsProfileOwner(string userName, string userId, ApplicationUserManager userManager)
	{
		var currentUser = await userManager.FindByNameAsync(userName);
		return currentUser.Id == userId;
	}

	private static async Task<bool> GetUserInRole(string userName, string roleName, int projectId, ApplicationUserManager userManager)
	{
		var user = await userManager.FindByNameAsync(userName);
		var isUserInRole = roleName == "Superadministrator"
			? await userManager.IsInRoleAsync(user, roleName)
			: await userManager.IsInRoleAsync(user, roleName, projectId);

		return isUserInRole;
	}
}
