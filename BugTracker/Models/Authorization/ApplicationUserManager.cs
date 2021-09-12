using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using BugTracker.Repository;

namespace BugTracker.Models.Authorization
{
	public class ApplicationUserManager : UserManager<IdentityUser>
	{
		private readonly string connectionString;

		public ApplicationUserManager(string connectionString) : this(new UserStore(connectionString), null, null, null, null, null, null, null, null)
		{
			this.connectionString = connectionString;
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
			var userRoleStore = new UserStore(connectionString);
			if(user == null)
			{
				throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "User ID not found: ", user.Id));
			}

			bool isAlreadyInRole = await userRoleStore.IsInRoleAsync(user, roleName, projectId, CancellationToken);
			if (isAlreadyInRole)
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
			var userRoleStore = new UserStore(connectionString);
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
			var userRoleStore = new UserStore(connectionString);
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
			var userRoleStore = new UserStore(connectionString);
			return await userRoleStore.GetUsersInRoleAsync(roleName, projectId, CancellationToken);
		}
	}
}
