using BugTracker.Models;
using BugTracker.Repository.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.Data;

namespace BugTracker.Repository.DapperRepositories
{
	public class DapperUserSubscriptionsRepository : DapperBaseRepository, IUserSubscriptionsRepository
	{
		public DapperUserSubscriptionsRepository(string connectionString) : base(connectionString) { }

		public void AddSubscription(int userId, int bugReportId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var insertedBugStateId = connection.ExecuteScalar("dbo.UserSubscriptions_Insert", new
				{
					UserId = userId,
					BugReportId = bugReportId
				},
					commandType: CommandType.StoredProcedure);
			}
		}

		public IEnumerable<BugReport> GetSubscribedReports(int userId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var bugReports = connection.Query<BugReport>("dbo.UserSubscriptions_GetAll @UserId", new { UserId = userId });
				return bugReports;
			}
		}

		public bool IsSubscribed(int userId, int bugReportId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var alreadySubscribed = connection.ExecuteScalar<bool>("dbo.UserSubscriptions_IsSubscribed", new { UserId = userId, BugReportId = bugReportId },
					commandType: CommandType.StoredProcedure);
				return alreadySubscribed;
			}
		}

		public void DeleteSubscription(int userId, int bugReportId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				connection.Execute("dbo.UserSubscriptions_Delete", new { UserId = userId, BugReportId = bugReportId },
					commandType: CommandType.StoredProcedure);
			}
		}

		public IEnumerable<int> GetAllSubscribedUserIds(int bugReportId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var userIds = connection.Query<int>("dbo.UserSubscriptions_GetUsersForReport", new { BugReportId = bugReportId },
					commandType: CommandType.StoredProcedure);
				return userIds;
			}
		}
	}
}
