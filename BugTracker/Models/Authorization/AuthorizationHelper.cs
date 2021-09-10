using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Authorization
{
	public static class AuthorizationHelper
	{
		public static bool UserIsSuperadministrator(string userName, string connectionString)
		{
			return GetUserInRole(userName, "Superadministrator", -1, connectionString).Result;
		}

		public static bool UserIsInProjectRole(string userName, string roleName, int id, string connectionString)
		{
			return GetUserInRole(userName, roleName, id, connectionString).Result;
		}

		public static async Task<bool> UserIsProfileOwner(string userName, int userId, string connectionString)
		{
			ApplicationUserManager userManager = new ApplicationUserManager(connectionString);
			var currentUser = await userManager.FindByNameAsync(userName);
			return Int32.Parse(currentUser.Id) == userId;
		}

		private static async Task<bool> GetUserInRole(string userName, string roleName, int id, string connectionString)
		{
			ApplicationUserManager userManager = new ApplicationUserManager(connectionString);
			var user = await userManager.FindByNameAsync(userName);
			bool userIsInRole;

			if (roleName == "Superadministrator")
			{
				userIsInRole = await userManager.IsInRoleAsync(user, roleName);
			}
			else
			{
				userIsInRole = await userManager.IsInRoleAsync(Int32.Parse(user.Id), roleName, id);
			}

			return userIsInRole;
		}
	}
}
