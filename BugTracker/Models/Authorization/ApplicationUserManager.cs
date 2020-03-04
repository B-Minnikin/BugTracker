using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Models.Database;

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

			var userRoles = await userRoleStore.GetRolesAsync(user, CancellationToken);
			if (userRoles.Contains(roleName))
			{
				IdentityError error = new IdentityError();
				error.Description = "User already in role";
				return IdentityResult.Failed(error);
			}
			await userRoleStore.AddToRoleAsync(user, roleName, projectId, CancellationToken);
			return await UpdateAsync(user);
		}

		public async Task<IdentityResult> RemoveFromRoleAsync(int userId, string roleName, int projectId)
		{
			ThrowIfDisposed();
			var userRoleStore = new UserStore();
			var user = await FindByIdAsync(userId.ToString());
			if(user == null)
			{
				throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "User ID not found: ", userId));
			}
			
			if(!await userRoleStore.IsInRoleAsync(user, roleName, projectId, CancellationToken))
			{
				return new IdentityResult();
			}

			await userRoleStore.RemoveFromRoleAsync(user, roleName, projectId, CancellationToken);
			return await UpdateAsync(user);
		}

		public async Task<bool> IsInRoleAsync(int userId, string roleName, int projectId)
		{
			ThrowIfDisposed();
			var userRoleStore = new UserStore();
			var user = await FindByIdAsync(userId.ToString());
			if(user == null)
			{
				throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "User ID not found: ", userId));
			}

			return await userRoleStore.IsInRoleAsync(user, roleName, projectId, CancellationToken);
		}

		public async Task<IList<IdentityUser>> GetUsersInRoleAsync(string roleName, int projectId)
		{
			ThrowIfDisposed();
			var userRoleStore = new UserStore();
			return await userRoleStore.GetUsersInRoleAsync(roleName, projectId, CancellationToken);
		}
	}
}
