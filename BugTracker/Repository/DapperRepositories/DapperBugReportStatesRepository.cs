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
	public class DapperBugReportStatesRepository : IBugReportStatesRepository
	{

		public BugState Add(BugState bugState)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
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
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				BugState currentState = connection.QueryFirst<BugState>("dbo.BugStates_GetLatest @BugReportId", new { BugReportId = bugReportId });
				return currentState;
			}
		}

		public BugState GetById(int bugStateId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				BugState bugState = connection.QueryFirst<BugState>("dbo.BugStates_GetById @BugStateId", new { BugStateId = bugStateId });
				return bugState;
			}
		}

		public IEnumerable<BugState> GetAllById(int bugReportId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var bugStates = connection.Query<BugState>("dbo.BugStates_GetAll @BugReportId", new { BugReportId = bugReportId });
				return bugStates;
			}
		}
	}
}
