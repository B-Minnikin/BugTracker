using BugTracker.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Repository.DapperRepositories
{
	public class DapperBugReportRepository : IBugReportRepository
	{
		public BugReport Add(BugReport bugReport)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var nextFreeId = connection.ExecuteScalar("dbo.LocalProjectBugReportIds_GetNextFreeId @ProjectId", new { ProjectId = bugReport.ProjectId });
				connection.Execute("dbo.LocalProjectBugReportIds_IncrementNextFreeId @ProjectId", new { ProjectId = bugReport.ProjectId });

				var insertedBugReportId = connection.ExecuteScalar("dbo.BugReports_Insert", new
				{
					Title = bugReport.Title,
					ProgramBehaviour = bugReport.ProgramBehaviour,
					DetailsToReproduce = bugReport.DetailsToReproduce,
					CreationTime = bugReport.CreationTime,
					Severity = bugReport.Severity,
					Importance = bugReport.Importance,
					PersonReporting = bugReport.PersonReporting,
					Hidden = bugReport.Hidden,
					ProjectId = bugReport.ProjectId,
					LocalBugReportId = nextFreeId
				},
					commandType: CommandType.StoredProcedure);
				BugReport insertedBugReport = connection.QueryFirst<BugReport>("dbo.BugReports_GetById @BugReportId", new { BugReportId = insertedBugReportId });
				return insertedBugReport;
			}
		}

		public int GetCommentCountById(int bugReportId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				int count = connection.ExecuteScalar<int>("dbo.BugReports_CommentCount @BugReportId", new { BugReportId = bugReportId });
				return count;
			}
		}

		public IEnumerable<BugReport> GetAllById(int projectId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var bugReports = connection.Query<BugReport>("dbo.BugReports_GetAll @ProjectId", new { ProjectId = projectId });
				return bugReports;
			}
		}

		public BugReport GetById(int bugReportId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var bugReport = connection.QueryFirst<BugReport>("dbo.BugReports_GetById @BugReportId", new { BugReportId = bugReportId });
				return bugReport;
			}
		}

		public BugReport GetBugReportByLocalId(int localBugReportId, int projectId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var bugReport = connection.QueryFirst<BugReport>("dbo.BugReports_GetByLocalId", new { LocalBugReportId = localBugReportId, ProjectId = projectId },
					commandType: CommandType.StoredProcedure);
				return bugReport;
			}
		}

		public BugReport Update(BugReport bugReportChanges)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				connection.ExecuteScalar("dbo.BugReports_Update", new
				{
					BugReportId = bugReportChanges.BugReportId,
					Title = bugReportChanges.Title,
					ProgramBehaviour = bugReportChanges.ProgramBehaviour,
					DetailsToReproduce = bugReportChanges.DetailsToReproduce,
					Severity = bugReportChanges.Severity,
					Importance = bugReportChanges.Importance,
					Hidden = bugReportChanges.Hidden
				}, commandType: CommandType.StoredProcedure);
				BugReport updatedBugReport = connection.QueryFirst<BugReport>("dbo.BugReports_GetById @BugReportId", new { BugReportId = bugReportChanges.BugReportId });
				return updatedBugReport;
			}
		}

		public BugReport Delete(int id)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var deletedBugReport = connection.QueryFirst<BugReport>("dbo.BugReports_GetById @BugReportId", new { BugReportId = id });
				connection.Execute("dbo.BugReports_DeleteById", new { BugReportId = id },
					commandType: CommandType.StoredProcedure);
				return deletedBugReport;
			}
		}
	}
}
