﻿using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class DapperProjectRepository : IProjectRepository
	{
		public Project Add(Project project)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var result = connection.Execute("dbo.Projects_Insert @Name @Description @CreationTime @LastUpdateTime @Hidden",
					new { Name = project.Name, Description = project.Description, CreationTime = project.CreationTime, LastUpdateTime = project.LastUpdateTime, Hidden = project.Hidden });
				var query = connection.QueryFirst<Project>("dbo.Projects_GetProject @ProjectId", new { ProjectId = result });
				return query;
			}
		}

		public BugReport AddBugReport(BugReport bugReport)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var insertedBugReportId = connection.ExecuteScalar("dbo.BugReports_Insert", new {
					Title = bugReport.Title, ProgramBehaviour = bugReport.ProgramBehaviour, DetailsToReproduce = bugReport.DetailsToReproduce, 
					CreationTime = bugReport.CreationTime, Severity = bugReport.Severity, Importance = bugReport.Importance, 
					PersonReporting = bugReport.PersonReporting, Hidden = bugReport.Hidden, ProjectId = bugReport.ProjectId },
					commandType: CommandType.StoredProcedure);
				BugReport insertedBugReport = connection.QueryFirst<BugReport>("dbo.BugReports_GetById @BugReportId", new { BugReportId = insertedBugReportId });
				return insertedBugReport;
			}
		}

		public Project Delete(int Id)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var deletedProject = connection.QueryFirst("dbo.Projects_GetById @ProjectId", new { ProjectId = Id });
				var result = connection.Execute("dbo.Projects_Delete @ProjectId", new { ProjectId = Id });

				return deletedProject;
			}
		}

		public IEnumerable<BugReport> GetAllBugReports(int ProjectId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var bugReports = connection.Query<BugReport>("dbo.BugReports_GetAll @ProjectId", new { ProjectId = ProjectId});
				return bugReports;
			}
		}

		public IEnumerable<Project> GetAllProjects()
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var query = connection.Query<Project>("dbo.Projects_GetAll");
				return query;
			}
		}

		public Project GetProject(int Id)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var output = connection.QueryFirst<Project>("dbo.Projects_GetById @ProjectId", new { ProjectId = Id });
				return output;
			}
		}

		public Project Update(Project projectChanges)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var output = connection.Execute("dbo.Projects_Update @ProjectId @Name @Description @CreationTime @LastUpdateTime @Hidden", projectChanges);
				var query = this.GetProject(output);
				return query;
			}
		}
	}
}
