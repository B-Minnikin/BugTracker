﻿using Dapper;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace BugTracker.Repository
{
	public class RoleStore : IRoleStore<IdentityRole>
	{
		public RoleStore(string connectionString)
		{
			this.connectionString = connectionString;
		}

		private System.Data.SqlClient.SqlConnection GetConnectionString()
		{
			return new System.Data.SqlClient.SqlConnection(connectionString);
		}

		public async Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			using (IDbConnection connection = GetConnectionString())
			{
				await connection.ExecuteScalarAsync("dbo.Roles_Insert", new
				{
					Name = role.Name,
					NormalizedName = role.NormalizedName
				},
					commandType: CommandType.StoredProcedure);
			}

			return IdentityResult.Success;
		}

		public async Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			using (IDbConnection connection = GetConnectionString())
			{
				await connection.ExecuteAsync("dbo.Roles_DeleteById @Id", new { Id = role.Id });
			}

			return IdentityResult.Success;
		}

		public async Task<IdentityRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			using (IDbConnection connection = GetConnectionString())
			{
				var role = await connection.QuerySingleOrDefaultAsync<IdentityRole>("dbo.Roles_FindById @RoleId", new { RoleId = roleId });
				return role;
			}
		}

		public async Task<IdentityRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			using (IDbConnection connection = GetConnectionString())
			{
				var role = await connection.QuerySingleOrDefaultAsync<IdentityRole>("dbo.Roles_FindByName @NormalizedName", new { NormalizedName = normalizedRoleName });
				return role;
			}
		}

		public Task<string> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
		{
			return Task.FromResult(role.NormalizedName);
		}

		public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken)
		{
			return Task.FromResult(role.Id.ToString());
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

			using (IDbConnection connection = GetConnectionString())
			{
				await connection.ExecuteAsync("dbo.Roles_Update", new
				{
					RoleId = role.Id,
					Name = role.Name,
					NormalizedName = role.NormalizedName
				}, commandType: CommandType.StoredProcedure);
			}

			return IdentityResult.Success;
		}

		public async Task<IdentityResult> UpdateAsync(IdentityRole role, int projectId, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			using (IDbConnection connection = GetConnectionString())
			{
				await connection.ExecuteAsync("dbo.Roles_Update", new
				{
					RoleId = role.Id,
					ProjectId = projectId,
					Name = role.Name,
					NormalizedName = role.NormalizedName
				}, commandType: CommandType.StoredProcedure);
			}

			return IdentityResult.Success;
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls
		private readonly string connectionString;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
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
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion

	}
}
