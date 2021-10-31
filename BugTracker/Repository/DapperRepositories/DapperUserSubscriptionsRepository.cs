using BugTracker.Models;
using BugTracker.Repository.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BugTracker.Repository.DapperRepositories
{
	public class DapperUserSubscriptionsRepository : DapperBaseRepository, IUserSubscriptionsRepository
	{
		public DapperUserSubscriptionsRepository(string connectionString) : base(connectionString) { }

		public async Task AddSubscription(int userId, int bugReportId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var insertedBugStateId = await connection.ExecuteScalarAsync("dbo.UserSubscriptions_Insert", new
				{
					UserId = userId,
					BugReportId = bugReportId
				},
					commandType: CommandType.StoredProcedure);
			}
		}

		public async Task<IEnumerable<BugReport>> GetSubscribedReports(int userId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var bugReports = await connection.QueryAsync<BugReport>("dbo.UserSubscriptions_GetAll @UserId", new { UserId = userId });
				return bugReports;
			}
		}

		public async Task<bool> IsSubscribed(int userId, int bugReportId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var alreadySubscribed = await connection.ExecuteScalarAsync<bool>("dbo.UserSubscriptions_IsSubscribed", new { UserId = userId, BugReportId = bugReportId },
					commandType: CommandType.StoredProcedure);
				return alreadySubscribed;
			}
		}

		public async Task DeleteSubscription(int userId, int bugReportId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				await connection.ExecuteAsync("dbo.UserSubscriptions_Delete", new { UserId = userId, BugReportId = bugReportId },
					commandType: CommandType.StoredProcedure);
			}
		}

		public async Task<IEnumerable<int>> GetAllSubscribedUserIds(int bugReportId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var userIds = await connection.QueryAsync<int>("dbo.UserSubscriptions_GetUsersForReport", new { BugReportId = bugReportId },
					commandType: CommandType.StoredProcedure);
				return userIds;
			}
		}
	}
}
