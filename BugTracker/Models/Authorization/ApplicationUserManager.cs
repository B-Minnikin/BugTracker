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
		private UserStore userStore;
		private string connectionString;

		public ApplicationUserManager(UserStore userStore, string connectionString) : this(userStore, null, null, null, null, null, null, null, null)
		{
			this.userStore = userStore;
			this.connectionString = connectionString;
		}

		public ApplicationUserManager(IUserStore<IdentityUser> store, IOptions<IdentityOptions> optionsAccessor,
			IPasswordHasher<IdentityUser> passwordHasher, IEnumerable<IUserValidator<IdentityUser>> userValidators,
			IEnumerable<IPasswordValidator<IdentityUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
			IServiceProvider services, ILogger<UserManager<IdentityUser>> logger) : base(store, optionsAccessor,
				passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
		{
		}

		public void RegisterConnectionString(string connectionString)
		{
			if(connectionString is null)
			{
				throw new ArgumentNullException(connectionString);
			}

			this.connectionString = connectionString;
		}

		public virtual void RegisterUserStore(UserStore userStore)
		{
			this.userStore = userStore;
		}

		public async Task<IdentityResult> AddToRoleAsync(IdentityUser user, string roleName, int projectId)
		{
			ThrowIfDisposed();
			if(user == null)
			{
				throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "User ID not found: ", user.Id));
			}

			bool isAlreadyInRole = await userStore.IsInRoleAsync(user, roleName, projectId, CancellationToken);
			if (isAlreadyInRole)
			{
				IdentityError error = new IdentityError();
				error.Description = "User already in role";
				return IdentityResult.Failed(error);
			}
			await userStore.AddToRoleAsync(user, roleName, projectId, CancellationToken);
			return await UpdateAsync(user);
		}

		public async Task<IdentityResult> RemoveFromRoleAsync(int userId, string roleName, int projectId)
		{
			ThrowIfDisposed();
			var user = await FindByIdAsync(userId.ToString());
			if(user == null)
			{
				throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "User ID not found: ", userId));
			}
			
			if(!await userStore.IsInRoleAsync(user, roleName, projectId, CancellationToken))
			{
				return new IdentityResult();
			}

			await userStore.RemoveFromRoleAsync(user, roleName, projectId, CancellationToken);
			return await UpdateAsync(user);
		}

		public async Task<bool> IsInRoleAsync(int userId, string roleName, int projectId)
		{
			ThrowIfDisposed();
			var user = await FindByIdAsync(userId.ToString());
			if(user == null)
			{
				throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "User ID not found: ", userId));
			}

			return await userStore.IsInRoleAsync(user, roleName, projectId, CancellationToken);
		}

		public async Task<IList<IdentityUser>> GetUsersInRoleAsync(string roleName, int projectId)
		{
			ThrowIfDisposed();
			return await userStore.GetUsersInRoleAsync(roleName, projectId, CancellationToken);
		}
	}
}
