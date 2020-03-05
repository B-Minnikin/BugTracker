using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Authorization
{
	public static class AuthorizationHelper
	{
		public static bool UserIsSuperadministrator(string userName)
		{
			return GetUserInRole(userName, "Superadministrator", -1).Result;
		}

		private static async Task<bool> GetUserInRole(string userName, string roleName, int id)
		{
			ApplicationUserManager userManager = new ApplicationUserManager();
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
