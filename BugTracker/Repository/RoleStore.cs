using System;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using BugTracker.Database.Context;
using BugTracker.Models.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Repository;

public sealed class RoleStore : IRoleStore<IdentityRole>
{
	private readonly ApplicationContext context;

	public RoleStore(ApplicationContext context)
	{
		this.context = context;
	}

	public async Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var appRole = new Role
		{
			Name = role.Name,
			NormalisedName = role.NormalizedName,
			IdentityRoleId = role.Id
		};

		context.Roles.Add(appRole);
		await context.SaveChangesAsync(cancellationToken);

		return IdentityResult.Success;
	}

	public async Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var appRole = await context.Roles
			.FirstOrDefaultAsync(r => r.Name == role.Name && r.IdentityRoleId == role.Id, cancellationToken);
		
		context.Roles.Remove(appRole);
		await context.SaveChangesAsync(cancellationToken);

		return IdentityResult.Success;
	}

	public async Task<IdentityRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var role = await context.Roles.FindAsync(new object[] { roleId, cancellationToken }, cancellationToken: cancellationToken);
		if (role is null) throw new NullReferenceException("Requested role was null");

		var identityRole = new IdentityRole
		{
			Id = role.IdentityRoleId,
			Name = role.Name,
			NormalizedName = role.NormalisedName
		};
		
		return identityRole;
	}

	public async Task<IdentityRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var role = await context.Roles.FirstOrDefaultAsync(r => r.NormalisedName == normalizedRoleName, cancellationToken);
		if (role is null) throw new NullReferenceException("Requested role was null");
		
		var identityRole = new IdentityRole
		{
			Id = role.IdentityRoleId,
			Name = role.Name,
			NormalizedName = role.NormalisedName,
		};

		return identityRole;
	}

	public Task<string> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
	{
		return Task.FromResult(role.NormalizedName);
	}

	public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken)
	{
		return Task.FromResult(role.Id);
	}

	public Task<string> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
	{
		return Task.FromResult(role.Name);
	}

	public Task SetNormalizedRoleNameAsync(IdentityRole role, string normalizedName, CancellationToken cancellationToken)
	{
		role.NormalizedName = normalizedName;
		return Task.FromResult(0);
	}

	public Task SetRoleNameAsync(IdentityRole role, string roleName, CancellationToken cancellationToken)
	{
		role.Name = roleName;
		return Task.FromResult(0);
	}

	// REMOVE -- use overloaded method taking projectId below
	public async Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		var appRole =
			await context.Roles.FirstOrDefaultAsync(r => r.IdentityRoleId == role.Id, cancellationToken);
		if (appRole is null) return IdentityResult.Failed();

		appRole.Name = role.Name;
		appRole.NormalisedName = role.NormalizedName;

		context.Roles.Update(appRole);
		await context.SaveChangesAsync(cancellationToken);

		return IdentityResult.Success;
	}

	public Task<IdentityResult> UpdateAsync(IdentityRole role, int projectId, CancellationToken cancellationToken)
	{
		// cancellationToken.ThrowIfCancellationRequested();
		//
		// using (IDbConnection connection = GetConnectionString())
		// {
		// 	await connection.ExecuteAsync("dbo.Roles_Update", new
		// 	{
		// 		RoleId = role.Id,
		// 		ProjectId = projectId,
		// 		Name = role.Name,
		// 		NormalizedName = role.NormalizedName
		// 	}, commandType: CommandType.StoredProcedure);
		// }
		//
		//return IdentityResult.Success;

		throw new NotImplementedException();
	}

	#region IDisposable Support
	private bool disposedValue; // To detect redundant calls

	private void Dispose(bool disposing)
	{
		if (disposedValue) return;
		if (disposing)
		{
		}

		disposedValue = true;
	}
	// ~RoleStore()
	// {
	//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
	//   Dispose(false);
	// }

	// This code added to correctly implement the disposable pattern.
	public void Dispose()
	{
		// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		Dispose(true);
		// GC.SuppressFinalize(this);
	}
	#endregion
}
