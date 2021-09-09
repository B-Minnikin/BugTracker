using BugTracker.Models;
using BugTracker.Repository.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Repository.DapperRepositories
{
	public class DapperBugReportStatesRepository : DapperBaseRepository, IBugReportStatesRepository
	{
		public DapperBugReportStatesRepository(string connectionString) : base(connectionString) { }

		public BugState Add(BugState bugState)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var insertedBugStateId = connection.ExecuteScalar("dbo.BugStates_Insert", new
				{
					Author = bugState.Author,
					Time = bugState.Time,
					StateType = bugState.StateType,
					BugReportId = bugState.BugReportId
				},
					commandType: CommandType.StoredProcedure);
				BugState insertedState = connection.QueryFirst<BugState>("dbo.BugStates_GetById @BugStateId", new { BugStateId = insertedBugStateId });
				return insertedState;
			}
		}

		public BugState GetLatestState(int bugReportId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				BugState currentState = connection.QueryFirst<BugState>("dbo.BugStates_GetLatest @BugReportId", new { BugReportId = bugReportId });
				return currentState;
			}
		}

		public BugState GetById(int bugStateId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				BugState bugState = connection.QueryFirst<BugState>("dbo.BugStates_GetById @BugStateId", new { BugStateId = bugStateId });
				return bugState;
			}
		}

		public IEnumerable<BugState> GetAllById(int bugReportId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var bugStates = connection.Query<BugState>("dbo.BugStates_GetAll @BugReportId", new { BugReportId = bugReportId });
				return bugStates;
			}
		}
	}
}
