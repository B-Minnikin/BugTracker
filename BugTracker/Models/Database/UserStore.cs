using Dapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BugTracker.Models.Database
{
	public class UserStore : IUserStore<IdentityUser>, IUserPasswordStore<IdentityUser>
	{
		public async Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				await connection.ExecuteScalarAsync("dbo.Users_Insert", new
				{
					UserName = user.UserName,
					NormalizedUserName = user.NormalizedUserName,
					Email = user.Email,
					NormalizedEmail = user.NormalizedEmail,
					PasswordHash = user.PasswordHash,
					PhoneNumber = user.PhoneNumber
				},
					commandType: CommandType.StoredProcedure);
			}

			return IdentityResult.Success;
		}

		public async Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				await connection.ExecuteAsync("dbo.Users_DeleteById @Id", new { Id = user.Id });
			}

			return IdentityResult.Success;
		}

		public async Task<IdentityUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var user = await connection.QueryFirstAsync<IdentityUser>("dbo.Users_FindById @UserId", new { UserId = userId });
				return user;
			}
		}

		public async Task<IdentityUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var user = await connection.QuerySingleOrDefaultAsync<IdentityUser>("dbo.Users_FindByName @NormalizedUserName", new { NormalizedUserName = normalizedUserName });
				return user;
			}
		}

		public Task<string> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.NormalizedUserName);
		}

		public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.Id.ToString());
		}

		public Task<string> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.UserName);
		}

		public Task SetNormalizedUserNameAsync(IdentityUser user, string normalizedName, CancellationToken cancellationToken)
		{
			user.NormalizedUserName = normalizedName;
			return Task.FromResult(0);
		}

		public Task SetUserNameAsync(IdentityUser user, string userName, CancellationToken cancellationToken)
		{
			user.UserName = userName;
			return Task.FromResult(0);
		}

		public async Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				await connection.ExecuteAsync("dbo.Users_Update", new
				{
					UserId = user.Id,
					UserName = user.UserName,
					NormalizedUserName = user.NormalizedUserName,
					Email = user.Email,
					NormalizedEmail = user.NormalizedEmail,
					PhoneNumber = user.PhoneNumber,
					PasswordHash = user.PasswordHash
				}, commandType: CommandType.StoredProcedure);
			}

			return IdentityResult.Success;
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

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

		public Task<string> GetPasswordHashAsync(IdentityUser user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.PasswordHash);
		}

		public Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken cancellationToken)
		{
			return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
		}

		public Task SetPasswordHashAsync(IdentityUser user, string passwordHash, CancellationToken cancellationToken)
		{
			user.PasswordHash = passwordHash;
			return Task.FromResult(0);
		}
		#endregion

	}
}
