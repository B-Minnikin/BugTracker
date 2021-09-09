using BugTracker.Models;
using BugTracker.Repository.DapperRepositories;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Repository
{
	public class DapperMilestoneRepository: DapperBaseRepository, IMilestoneRepository
	{
		public DapperMilestoneRepository(string connectionString) : base(connectionString) { }

		public Milestone Add(Milestone milestone)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				int insertedMilestoneId = (Int32)connection.ExecuteScalar("dbo.Milestones_Insert", new
				{
					ProjectId = milestone.ProjectId,
					Title = milestone.Title,
					Description = milestone.Description,
					CreationDate = milestone.CreationTime,
					DueDate = milestone.DueDate
				},
					commandType: CommandType.StoredProcedure);
				Milestone insertedMilestone = this.GetById(insertedMilestoneId);
				return insertedMilestone;
			}
		}

		public Milestone Delete(int milestoneId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var deletedMilestone = connection.QueryFirst<Milestone>("dbo.Milestones_GetById @MilestoneId", new { MilestoneId = milestoneId });
				connection.Execute("dbo.Milestones_DeleteById", new { MilestoneId = milestoneId },
					commandType: CommandType.StoredProcedure);

				return deletedMilestone;
			}
		}

		public Milestone Update(Milestone milestone)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var projectId = connection.ExecuteScalar("dbo.Milestones_Update", new
				{
					MilestoneId = milestone.MilestoneId,
					ProjectId = milestone.ProjectId,
					Title = milestone.Title,
					Description = milestone.Description,
					CreationDate = milestone.CreationTime,
					DueDate = milestone.DueDate
				}, commandType: CommandType.StoredProcedure);

				Milestone updatedMilestone = this.GetById(milestone.MilestoneId);
				return updatedMilestone;
			}
		}

		public IEnumerable<Milestone> GetAllById(int projectId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var milestones = connection.Query<Milestone>("dbo.Milestones_GetAll", new { ProjectId = projectId },
					commandType: CommandType.StoredProcedure);
				return milestones;
			}
		}

		public Milestone GetById(int id)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var milestone = connection.QueryFirst<Milestone>("dbo.Milestones_GetById @MilestoneId", new { MilestoneId = id });
				return milestone;
			}
		}

		public void AddMilestoneBugReport(int milestoneId, int bugReportId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				connection.Execute("dbo.MilestoneBugReports_Insert", new { MilestoneId = milestoneId, BugReportId = bugReportId },
					commandType: CommandType.StoredProcedure);
			}
		}

		public void RemoveMilestoneBugReport(int milestoneId, int bugReportId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				connection.Execute("dbo.MilestoneBugReports_Delete", new { MilestoneId = milestoneId, BugReportId = bugReportId },
					commandType: CommandType.StoredProcedure);
			}
		}

		public IEnumerable<MilestoneBugReportEntry> GetMilestoneBugReportEntries(int milestoneId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var milestoneBugReportIds = connection.Query<MilestoneBugReportEntry>("dbo.MilestoneBugReports_GetAllReportEntries", new { MilestoneId = milestoneId },
					commandType: CommandType.StoredProcedure);
				return milestoneBugReportIds;
			}
		}

		public IEnumerable<BugReport> GetMilestoneBugReports(int milestoneId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var milestoneBugReports = connection.Query<BugReport>("dbo.MilestoneBugReports_GetAllReports", new { MilestoneId = milestoneId },
					commandType: CommandType.StoredProcedure);
				return milestoneBugReports;
			}
		}
	}
}
