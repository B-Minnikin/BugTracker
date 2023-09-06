using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using BugTracker.Database.Repository.Interfaces;
using BugTracker.Models;
using Dapper;

namespace BugTracker.Database.Repository.DapperRepositories
{
	public class DapperMilestoneRepository: DapperBaseRepository, IMilestoneRepository
	{
		public DapperMilestoneRepository(string connectionString) : base(connectionString) { }

		public async Task<Milestone> Add(Milestone milestone)
		{
			using IDbConnection connection = GetConnectionString();
			var insertedMilestoneId = (int) await connection.ExecuteScalarAsync("dbo.Milestones_Insert", new
				{
					milestone.ProjectId,
					milestone.Title,
					milestone.Description,
					CreationDate = milestone.CreationTime,
					milestone.DueDate
				},
				commandType: CommandType.StoredProcedure);
			Milestone insertedMilestone = await this.GetById(insertedMilestoneId);
			return insertedMilestone;
		}

		public async Task<Milestone> Delete(int milestoneId)
		{
			using IDbConnection connection = GetConnectionString();
			var deletedMilestone = await connection.QueryFirstAsync<Milestone>("dbo.Milestones_GetById @MilestoneId", new { MilestoneId = milestoneId });
			await connection.ExecuteAsync("dbo.Milestones_DeleteById", new { MilestoneId = milestoneId },
				commandType: CommandType.StoredProcedure);

			return deletedMilestone;
		}

		public async Task<Milestone> Update(Milestone milestone)
		{
			using IDbConnection connection = GetConnectionString();
			_ = await connection.ExecuteScalarAsync("dbo.Milestones_Update", new
			{
				milestone.MilestoneId,
				milestone.ProjectId,
				milestone.Title,
				milestone.Description,
				CreationDate = milestone.CreationTime,
				milestone.DueDate
			}, commandType: CommandType.StoredProcedure);

			var updatedMilestone = await GetById(milestone.MilestoneId);
			return updatedMilestone;
		}

		public async Task<IEnumerable<Milestone>> GetAllById(int projectId)
		{
			using IDbConnection connection = GetConnectionString();
			var milestones = await connection.QueryAsync<Milestone>("dbo.Milestones_GetAll", new { ProjectId = projectId },
				commandType: CommandType.StoredProcedure);
			return milestones;
		}

		public async Task<Milestone> GetById(int id)
		{
			using IDbConnection connection = GetConnectionString();
			var milestone = await connection.QueryFirstAsync<Milestone>("dbo.Milestones_GetById @MilestoneId", new { MilestoneId = id });
			return milestone;
		}

		public async Task AddMilestoneBugReport(int milestoneId, int bugReportId)
		{
			using IDbConnection connection = GetConnectionString();
			await connection.ExecuteAsync("dbo.MilestoneBugReports_Insert", new { MilestoneId = milestoneId, BugReportId = bugReportId },
				commandType: CommandType.StoredProcedure);
		}

		public async Task RemoveMilestoneBugReport(int milestoneId, int bugReportId)
		{
			using IDbConnection connection = GetConnectionString();
			await connection.ExecuteAsync("dbo.MilestoneBugReports_Delete", new { MilestoneId = milestoneId, BugReportId = bugReportId },
				commandType: CommandType.StoredProcedure);
		}

		public async Task<IEnumerable<MilestoneBugReportEntry>> GetMilestoneBugReportEntries(int milestoneId)
		{
			using IDbConnection connection = GetConnectionString();
			var milestoneBugReportIds = await connection.QueryAsync<MilestoneBugReportEntry>("dbo.MilestoneBugReports_GetAllReportEntries", new { MilestoneId = milestoneId },
				commandType: CommandType.StoredProcedure);
			return milestoneBugReportIds;
		}

		public async Task<IEnumerable<BugReport>> GetMilestoneBugReports(int milestoneId)
		{
			using IDbConnection connection = GetConnectionString();
			var milestoneBugReports = await connection.QueryAsync<BugReport>("dbo.MilestoneBugReports_GetAllReports", new { MilestoneId = milestoneId },
				commandType: CommandType.StoredProcedure);
			return milestoneBugReports;
		}
	}
}
