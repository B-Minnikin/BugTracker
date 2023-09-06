using BugTracker.Models;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BugTracker.Database.Repository.Interfaces;

namespace BugTracker.Database.Repository.DapperRepositories
{
	public class DapperBugReportRepository : DapperBaseRepository, IBugReportRepository
	{
		public DapperBugReportRepository(string connectionString) : base(connectionString) { }

		public async Task<BugReport> Add(BugReport bugReport)
		{
			using IDbConnection connection = GetConnectionString();
			var nextFreeId = await connection.ExecuteScalarAsync("dbo.LocalProjectBugReportIds_GetNextFreeId @ProjectId", new { bugReport.ProjectId });
			await connection.ExecuteAsync("dbo.LocalProjectBugReportIds_IncrementNextFreeId @ProjectId", new { bugReport.ProjectId });

			var insertedBugReportId = await connection.ExecuteScalarAsync("dbo.BugReports_Insert", new
				{
					bugReport.Title,
					bugReport.ProgramBehaviour,
					bugReport.DetailsToReproduce,
					bugReport.CreationTime,
					bugReport.Severity,
					bugReport.Importance,
					bugReport.PersonReporting,
					bugReport.Hidden,
					bugReport.ProjectId,
					LocalBugReportId = nextFreeId
				},
				commandType: CommandType.StoredProcedure);
			var insertedBugReport = await connection.QueryFirstAsync<BugReport>("dbo.BugReports_GetById @BugReportId", new { BugReportId = insertedBugReportId });
			return insertedBugReport;
		}

		public int GetCommentCountById(int bugReportId)
		{
			using IDbConnection connection = GetConnectionString();
			var count = connection.ExecuteScalar<int>("dbo.BugReports_CommentCount @BugReportId", new { BugReportId = bugReportId });
			return count;
		}

		public async Task<IEnumerable<BugReport>> GetAllById(int projectId)
		{
			using IDbConnection connection = GetConnectionString();
			var bugReports = await connection.QueryAsync<BugReport>("dbo.BugReports_GetAll @ProjectId", new { ProjectId = projectId });
			return bugReports;
		}

		public async Task<BugReport> GetById(int bugReportId)
		{
			using IDbConnection connection = GetConnectionString();
			var bugReport = await connection.QueryFirstAsync<BugReport>("dbo.BugReports_GetById @BugReportId", new { BugReportId = bugReportId });
			return bugReport;
		}

		public async Task<BugReport> GetBugReportByLocalId(int localBugReportId, int projectId)
		{
			using IDbConnection connection = GetConnectionString();
			var bugReport = await connection.QueryFirstAsync<BugReport>("dbo.BugReports_GetByLocalId", new { LocalBugReportId = localBugReportId, ProjectId = projectId },
				commandType: CommandType.StoredProcedure);
			return bugReport;
		}

		public async Task<BugReport> Update(BugReport bugReportChanges)
		{
			using IDbConnection connection = GetConnectionString();
			await connection.ExecuteScalarAsync("dbo.BugReports_Update", new
			{
				bugReportChanges.BugReportId,
				bugReportChanges.Title,
				bugReportChanges.ProgramBehaviour,
				bugReportChanges.DetailsToReproduce,
				bugReportChanges.Severity,
				bugReportChanges.Importance,
				bugReportChanges.Hidden
			}, commandType: CommandType.StoredProcedure);
			var updatedBugReport = await connection.QueryFirstAsync<BugReport>("dbo.BugReports_GetById @BugReportId", new { bugReportChanges.BugReportId });
			return updatedBugReport;
		}

		public async Task<BugReport> Delete(int id)
		{
			using IDbConnection connection = GetConnectionString();
			var deletedBugReport = await connection.QueryFirstAsync<BugReport>("dbo.BugReports_GetById @BugReportId", new { BugReportId = id });
			connection.Execute("dbo.BugReports_DeleteById", new { BugReportId = id },
				commandType: CommandType.StoredProcedure);
			return deletedBugReport;
		}

		public async Task<IEnumerable<AttachmentPath>> GetAttachmentPaths(AttachmentParentType parentType, int parentId)
		{
			using IDbConnection connection = GetConnectionString();
			string procedure;

			switch (parentType)
			{
				case AttachmentParentType.BugReport:
					procedure = "dbo.AttachmentPaths_BugReport_GetAll @ParentId";
					break;
				case AttachmentParentType.Comment:
					procedure = "dbo.AttachmentPaths_Comment_GetAll @ParentId";
					break;
				default:
					throw new System.ArgumentException("Parameter must be a valid type", "parentType");
			}

			var attachmentPaths = await connection.QueryAsync<AttachmentPath>(procedure, new { ParentId = parentId });
			return attachmentPaths;
		}

		public async Task AddLocalBugReportId(int projectId)
		{
			using IDbConnection connection = GetConnectionString();
			await connection.ExecuteAsync("dbo.LocalProjectBugReportIds_Insert", new { ProjectId = projectId },
				commandType: CommandType.StoredProcedure);
		}

		public async Task AddUserAssignedToBugReport(string userId, int bugReportId)
		{
			using IDbConnection connection = GetConnectionString();
			await connection.ExecuteAsync("dbo.UsersAssignedToBugReport_Insert", new { UserId = userId, BugReportId = bugReportId },
				commandType: CommandType.StoredProcedure);
		}

		public async Task DeleteUserAssignedToBugReport(string userId, int bugReportId)
		{
			using IDbConnection connection = GetConnectionString();
			await connection.ExecuteAsync("dbo.UsersAssignedToBugReport_Delete", new { UserId = userId, BugReportId = bugReportId },
				commandType: CommandType.StoredProcedure);
		}

		public async Task<IEnumerable<BugReport>> GetBugReportsForAssignedUser(string userId)
		{
			using IDbConnection connection = GetConnectionString();
			var bugReports = await connection.QueryAsync<BugReport>("dbo.UsersAssignedToBugReport_GetReportsForUser", new { UserId = userId },
				commandType: CommandType.StoredProcedure);
			return bugReports;
		}

		public async Task<IEnumerable<ApplicationUser>> GetAssignedUsersForBugReport(int bugReportId)
		{
			using IDbConnection connection = GetConnectionString();
			var users = await connection.QueryAsync<ApplicationUser>("dbo.UsersAssignedToBugReport_GetUsersForReport", new { BugReportId = bugReportId },
				commandType: CommandType.StoredProcedure);
			return users;
		}

		public async Task AddBugReportLink(int bugReportId, int linkToBugReportId)
		{
			using IDbConnection connection = GetConnectionString();
			await connection.ExecuteAsync("dbo.BugReports_InsertLink", new { BugReportId = bugReportId, LinkToBugReportId = linkToBugReportId },
				commandType: CommandType.StoredProcedure);
		}

		public async Task DeleteBugReportLink(int bugReportId, int linkToBugReportId)
		{
			using IDbConnection connection = GetConnectionString();
			await connection.ExecuteAsync("dbo.BugReports_DeleteLink", new { BugReportId = bugReportId, LinkToBugReportId = linkToBugReportId },
				commandType: CommandType.StoredProcedure);
		}

		public async Task<IEnumerable<BugReport>> GetLinkedReports(int bugReportId)
		{
			using IDbConnection connection = GetConnectionString();
			var bugReports = await connection.QueryAsync<BugReport>("dbo.BugReports_GetLinkedReports", new { BugReportId = bugReportId },
				commandType: CommandType.StoredProcedure);
			return bugReports;
		}
	}
}
