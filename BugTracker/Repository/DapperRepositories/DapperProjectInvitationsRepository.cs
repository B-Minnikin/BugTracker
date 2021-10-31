using BugTracker.Repository.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BugTracker.Repository.DapperRepositories
{
	public class DapperProjectInvitationsRepository : DapperBaseRepository, IProjectInvitationsRepository
	{
		public DapperProjectInvitationsRepository(string connectionString) : base(connectionString) { }

		public async Task AddPendingProjectInvitation(string emailAddress, int projectId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				await connection.ExecuteAsync("dbo.ProjectInvitations_Insert", new { EmailAddress = emailAddress, ProjectId = projectId },
					commandType: CommandType.StoredProcedure);
			}
		}

		public async Task DeletePendingProjectInvitation(string emailAddress, int projectId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				await connection.ExecuteAsync("dbo.ProjectInvitations_Delete", new { EmailAddress = emailAddress, ProjectId = projectId },
					commandType: CommandType.StoredProcedure);
			}
		}

		public async Task<bool> IsEmailAddressPendingRegistration(string emailAddress, int projectId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var pendingRegistration = await connection.ExecuteScalarAsync<bool>("dbo.ProjectInvitations_IsPendingRegistration", new { EmailAddress = emailAddress, ProjectId = projectId },
					commandType: CommandType.StoredProcedure);
				return pendingRegistration;
			}
		}

		public async Task<IEnumerable<int>> GetProjectInvitationsForEmailAddress(string emailAddress)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var projectIds = await connection.QueryAsync<int>("dbo.ProjectInvitations_GetInvitationsForEmailAddress", new { EmailAddress = emailAddress },
					commandType: CommandType.StoredProcedure);
				return projectIds;
			}
		}
	}
}
