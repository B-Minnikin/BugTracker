using BugTracker.Repository;
using System.Threading.Tasks;

namespace BugTracker.Models.Authorization;

public static class AuthorizationHelper
{
	public static Task<bool> UserIsSuperadministrator(string userName, string connectionString)
	{
		return GetUserInRole(userName, "Superadministrator", -1, connectionString);
	}

	// TODO - here
	public static async Task<bool> UserIsInProjectRole(string userName, string roleName, int id, string connectionString)
	{
		return await GetUserInRole(userName, roleName, id, connectionString);
	}

	public static async Task<bool> UserIsProfileOwner(string userName, int userId, string connectionString)
	{
		var userManager = new ApplicationUserManager(new UserStore(connectionString), connectionString);
		var currentUser = await userManager.FindByNameAsync(userName);
		return int.Parse(currentUser.Id) == userId;
	}

	private static async Task<bool> GetUserInRole(string userName, string roleName, int id, string connectionString)
	{
		var userManager = new ApplicationUserManager(new UserStore(connectionString), connectionString);
		var user = await userManager.FindByNameAsync(userName);
		
		return roleName == "Superadministrator"
			? await userManager.IsInRoleAsync(user, roleName)
			: await userManager.IsInRoleAsync(int.Parse(user.Id), roleName, id);
	}
}
