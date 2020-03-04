using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Authorization
{
	public class ApplicationUserManager : UserManager<IdentityUser>
	{
		public ApplicationUserManager() : this(new UserStore(), null, null, null, null, null, null, null, null)
		{
		}

		public ApplicationUserManager(IUserStore<IdentityUser> store, IOptions<IdentityOptions> optionsAccessor,
			IPasswordHasher<IdentityUser> passwordHasher, IEnumerable<IUserValidator<IdentityUser>> userValidators,
			IEnumerable<IPasswordValidator<IdentityUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
			IServiceProvider services, ILogger<UserManager<IdentityUser>> logger) : base(store, optionsAccessor,
				passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
		{
		}

		public async Task<IdentityResult> AddToRoleAsync(IdentityUser user, string roleName, int projectId)
		{
			ThrowIfDisposed();
			var userRoleStore = new UserStore();
			if(user == null)
			{
				throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "User ID not found: ", user.Id));
			}

			var userRoles = await userRoleStore.GetUserRolesAsync(user, CancellationToken);
			if (userRoles.Contains(roleName))
			{
				IdentityError error = new IdentityError();
				error.Description = "User already in role";
				return IdentityResult.Failed(error);
			}
			await userRoleStore.AddToRoleAsync(user, roleName, projectId, CancellationToken);
			return await UpdateAsync(user);
		}
	}
}
