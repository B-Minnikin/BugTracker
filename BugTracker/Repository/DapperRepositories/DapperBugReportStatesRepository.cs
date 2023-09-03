using BugTracker.Models;
using BugTracker.Repository.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BugTracker.Repository.DapperRepositories
{
	public class DapperBugReportStatesRepository : DapperBaseRepository, IBugReportStatesRepository
	{
		public DapperBugReportStatesRepository(string connectionString) : base(connectionString) { }

		public async Task<BugState> Add(BugState bugState)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var insertedBugStateId = await connection.ExecuteScalarAsync("dbo.BugStates_Insert", new
				{
					bugState.Author,
					bugState.Time,
					bugState.StateType,
					bugState.BugReportId
				},
					commandType: CommandType.StoredProcedure);
				BugState insertedState = await connection.QueryFirstAsync<BugState>("dbo.BugStates_GetById @BugStateId", new { BugStateId = insertedBugStateId });
				return insertedState;
			}
		}

		public async Task<BugState> GetLatestState(int bugReportId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				BugState currentState = await connection.QueryFirstAsync<BugState>("dbo.BugStates_GetLatest @BugReportId", new { BugReportId = bugReportId });
				return currentState;
			}
		}

		public async Task<BugState> GetById(int bugStateId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				BugState bugState = await connection.QueryFirstAsync<BugState>("dbo.BugStates_GetById @BugStateId", new { BugStateId = bugStateId });
				return bugState;
			}
		}

		public async Task<IEnumerable<BugState>> GetAllById(int bugReportId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var bugStates = await connection.QueryAsync<BugState>("dbo.BugStates_GetAll @BugReportId", new { BugReportId = bugReportId });
				return bugStates;
			}
		}
	}
}
