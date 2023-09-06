using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BugTracker.Database.Repository.Interfaces;
using BugTracker.Models;
using Dapper;

namespace BugTracker.Database.Repository.DapperRepositories;

public class DapperUserSubscriptionsRepository : DapperBaseRepository, IUserSubscriptionsRepository
{
	public DapperUserSubscriptionsRepository(string connectionString) : base(connectionString) { }

	public async Task AddSubscription(string userId, int bugReportId)
	{
		using IDbConnection connection = GetConnectionString();
		_ = await connection.ExecuteScalarAsync("dbo.UserSubscriptions_Insert", new
			{
				UserId = userId,
				BugReportId = bugReportId
			},
			commandType: CommandType.StoredProcedure);
	}

	public async Task<IEnumerable<BugReport>> GetSubscribedReports(string userId)
	{
		using IDbConnection connection = GetConnectionString();
		var bugReports = await connection.QueryAsync<BugReport>("dbo.UserSubscriptions_GetAll @UserId", new { UserId = userId });
		return bugReports;
	}

	public async Task<bool> IsSubscribed(string userId, int bugReportId)
	{
		using IDbConnection connection = GetConnectionString();
		var alreadySubscribed = await connection.ExecuteScalarAsync<bool>("dbo.UserSubscriptions_IsSubscribed", new { UserId = userId, BugReportId = bugReportId },
			commandType: CommandType.StoredProcedure);
		return alreadySubscribed;
	}

	public async Task DeleteSubscription(string userId, int bugReportId)
	{
		using IDbConnection connection = GetConnectionString();
		await connection.ExecuteAsync("dbo.UserSubscriptions_Delete", new { UserId = userId, BugReportId = bugReportId },
			commandType: CommandType.StoredProcedure);
	}

	public async Task<IEnumerable<string>> GetAllSubscribedUserIds(int bugReportId)
	{
		using IDbConnection connection = GetConnectionString();
		var userIds = await connection.QueryAsync<int>("dbo.UserSubscriptions_GetUsersForReport", new { BugReportId = bugReportId },
			commandType: CommandType.StoredProcedure);
		return default; // broken for now
	}
}
