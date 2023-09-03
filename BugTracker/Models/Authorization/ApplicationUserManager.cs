using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BugTracker.Database.Context;
using BugTracker.Repository;

namespace BugTracker.Models.Authorization;
	
public class ApplicationUserManager : UserManager<ApplicationUser>
{
	private readonly ApplicationContext context;

	public ApplicationUserManager(ApplicationContext context) : this(new UserStore(context), null, null, null, null, null, null, null, null)
	{
		this.context = context;
	}
	
	public ApplicationUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor,
		IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators,
		IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
		IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger) : base(store, optionsAccessor,
			passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
	{}

	public ApplicationUserManager(ApplicationContext context, IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor,
		IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators,
		IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors,
		IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger) : base(store, optionsAccessor,
		passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
	{
		this.context = context;
	}

	public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string roleName, int projectId)
	{
		ThrowIfDisposed();
		var userRoleStore = new UserStore(context);
		if(user == null)
		{
			throw new ArgumentNullException(nameof(user));
		}

		var isAlreadyInRole = await userRoleStore.IsInRoleAsync(user, roleName, projectId, CancellationToken);
		if (isAlreadyInRole)
		{
			var error = new IdentityError
			{
				Description = "User already in role"
			};
			return IdentityResult.Failed(error);
		}
		await userRoleStore.AddToRoleAsync(user, roleName, projectId, CancellationToken);
		return await UpdateAsync(user);
	}

	public async Task<IdentityResult> RemoveFromRoleAsync(string userId, string roleName, int projectId)
	{
		ThrowIfDisposed();
		var userRoleStore = new UserStore(context);
		var user = await FindByIdAsync(userId);
		if(user == null)
		{
			throw new ArgumentNullException(nameof(user));
		}
		
		if(!await userRoleStore.IsInRoleAsync(user, roleName, projectId, CancellationToken))
		{
			return new IdentityResult();
		}

		await userRoleStore.RemoveFromRoleAsync(user, roleName, projectId, CancellationToken);
		return await UpdateAsync(user);
	}

	public async Task<bool> IsInRoleAsync(string userId, string roleName, int projectId)
	{
		ThrowIfDisposed();
		var userRoleStore = new UserStore(context);
		var user = await FindByIdAsync(userId);
		if(user == null)
		{
			throw new InvalidOperationException($"User ID not found: {userId}");
		}

		return await userRoleStore.IsInRoleAsync(user, roleName, projectId, CancellationToken);
	}

	public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, int projectId)
	{
		ThrowIfDisposed();
		var userRoleStore = new UserStore(context);
		return await userRoleStore.GetUsersInRoleAsync(roleName, projectId, CancellationToken);
	}
}
