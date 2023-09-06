using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BugTracker.Database.Context;
using BugTracker.Models;
using BugTracker.Models.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Database.Repository;

public class UserStore : IUserPasswordStore<ApplicationUser>, IUserEmailStore<ApplicationUser>, IUserRoleStore<ApplicationUser>
{
	private readonly ApplicationContext context;

	public UserStore(ApplicationContext context)
	{
		this.context = context;
	}
	
	public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		if (user is null) return IdentityResult.Failed();
		cancellationToken.ThrowIfCancellationRequested();

		context.Users.Add(user);
		await context.SaveChangesAsync(cancellationToken);

		return IdentityResult.Success;
	}

	public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		if (user is null) return IdentityResult.Failed();
		cancellationToken.ThrowIfCancellationRequested();

		user.Hidden = true;
		context.Users.Update(user);
		await context.SaveChangesAsync(cancellationToken);

		return IdentityResult.Success;
	}

	public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var user = await context.Users.FindAsync(new object[] { userId, cancellationToken }, cancellationToken: cancellationToken);
		return user;
	}

	public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var user = await context.Users
			.FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName, cancellationToken);
		return user;
	}

	public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user.NormalizedUserName);
	}

	public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user.Id);
	}

	public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user.UserName);
	}

	public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
	{
		user.NormalizedUserName = normalizedName;
		return Task.FromResult(0);
	}

	public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
	{
		user.UserName = userName;
		return Task.FromResult(0);
	}

	public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		if (user is null) return IdentityResult.Failed();
		cancellationToken.ThrowIfCancellationRequested();

		user.Hidden = true;
		context.Users.Update(user);
		await context.SaveChangesAsync(cancellationToken);
		
		return IdentityResult.Success;
	}

	#region IDisposable Support

	private bool disposedValue; // To detect redundant calls

	protected virtual void Dispose(bool disposing)
	{
		if (disposedValue) return;
		if (disposing)
		{
			// TODO: dispose managed state (managed objects).
		}

		// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
		// TODO: set large fields to null.

		disposedValue = true;
	}

	// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
	// ~UserStore()
	// {
	//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
	//   Dispose(false);
	// }

	// This code added to correctly implement the disposable pattern.
	public void Dispose()
	{
		// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		Dispose(true);
		// TODO: uncomment the following line if the finalizer is overridden above.
		// GC.SuppressFinalize(this);
	}

	public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user.PasswordHash);
	}

	public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
	}

	public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
	{
		user.PasswordHash = passwordHash;
		return Task.FromResult(0);
	}

	public async Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var user = await context.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
		return user;
	}

	public Task<string> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user.Email);
	}

	public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user.EmailConfirmed);
	}

	public Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		return Task.FromResult(user.NormalizedEmail);
	}

	public Task SetEmailAsync(ApplicationUser user, string email, CancellationToken cancellationToken)
	{
		user.Email = email;
		return Task.FromResult(0);
	}

	public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
	{
		user.EmailConfirmed = confirmed;
		return Task.FromResult(0);
	}

	public Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken cancellationToken)
	{
		user.NormalizedEmail = normalizedEmail;
		return Task.FromResult(0);
	}

	public Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public async Task AddToRoleAsync(ApplicationUser user, string roleName, int projectId, CancellationToken cancellationToken)
	{
		if (user is null) throw new ArgumentNullException(nameof(user));
		cancellationToken.ThrowIfCancellationRequested();

		var role = await context.Roles.FirstOrDefaultAsync(r => r.NormalisedName == roleName, cancellationToken);
		if (role is null)
		{
			// create a new role with the required name
			role = new Role
			{
				Name = roleName,
				NormalisedName = roleName.ToUpper(),
			};

			context.Roles.Add(role);
			await context.SaveChangesAsync(cancellationToken);
		}

		var userRole = new UserRole
		{
			RoleId = role.Id,
			UserId = user.Id,
			ProjectId = projectId
		};

		context.UserRoles.Add(userRole);
		await context.SaveChangesAsync(cancellationToken);
	}

	public async Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
	{
		if (user is null) throw new ArgumentNullException(nameof(user));
		cancellationToken.ThrowIfCancellationRequested();

		var roles = await context.UserRoles
			.Where(ur => ur.UserId == user.Id)
			.Join(context.Roles,
				ur => ur.RoleId,
				r => r.Id,
				(ur, r) => r.Name)
			.ToListAsync(cancellationToken);

		return roles;
	}

	public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var users = await context.UserRoles.Where(ur => ur.Role.NormalisedName == roleName.ToUpper())
			.Join(context.Users,
				ur => ur.UserId,
				u => u.Id,
				(ur, u) => u).ToListAsync(cancellationToken);

		return users;
	}

	public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, int projectId, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var users = await context.UserRoles.Where(ur => ur.Role.NormalisedName == roleName.ToUpper() && ur.ProjectId == projectId)
			.Join(context.Users,
				ur => ur.UserId,
				u => u.Id,
				(ur, u) => u).ToListAsync(cancellationToken);

		return users;
	}

	public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var role = await context.UserRoles.Where(ur => ur.UserId == user.Id)
			.Join(context.Roles,
				ur => ur.RoleId,
				r => r.Id,
				(ur, r) => new { UserRole = ur, Role = r })
			.FirstOrDefaultAsync(cancellationToken);

		return role != null;
	}

	public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, int projectId, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var role = await context.UserRoles.Where(ur => ur.UserId == user.Id && ur.ProjectId == projectId)
			.Join(context.Roles,
				ur => ur.RoleId,
				r => r.Id,
				(ur, r) => new { UserRole = ur, Role = r })
			.FirstOrDefaultAsync(cancellationToken);

		return role != null;
	}

	public Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public async Task RemoveFromRoleAsync(ApplicationUser user, string roleName, int projectId, CancellationToken cancellationToken)
	{
		if (user is null) throw new ArgumentNullException(nameof(user));
		cancellationToken.ThrowIfCancellationRequested();

		var userRole = await context.UserRoles
			.Where(ur => ur.UserId == user.Id && ur.ProjectId == projectId)
			.Join(context.Roles,
				ur => ur.RoleId,
				r => r.Id,
				(ur, r) => ur)
			.FirstOrDefaultAsync(cancellationToken);

		context.UserRoles.Remove(userRole);
		await context.SaveChangesAsync(cancellationToken);
	}
	#endregion
}
