using BugTracker.Extension_Methods;
using BugTracker.Repository.DapperRepositories;
using Dapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class DapperProjectRepository : DapperBaseRepository, IProjectRepository
	{
		public DapperProjectRepository(string connectionString) : base(connectionString) { }

		public Project Add(Project project)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var insertedProjectId = connection.ExecuteScalar("dbo.Projects_Insert", new {
					Name = project.Name, Description = project.Description, CreationTime = project.CreationTime,
					LastUpdateTime = project.LastUpdateTime, Hidden = project.Hidden },
					commandType: CommandType.StoredProcedure);
				var insertedProject = connection.QueryFirst<Project>("dbo.Projects_GetById @ProjectId", new { ProjectId = insertedProjectId });
				return insertedProject;
			}
		}

		public Project Delete(int id)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var deletedProject = connection.QueryFirst<Project>("dbo.Projects_GetById @ProjectId", new { ProjectId = id });
				var result = connection.Execute("dbo.Projects_Delete @ProjectId", new { ProjectId = id });

				return deletedProject;
			}
		}

		public IEnumerable<Project> GetAll()
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var projects = connection.Query<Project>("dbo.Projects_GetAll");
				return projects;
			}
		}

		public Project GetById(int id)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var project = connection.QueryFirst<Project>("dbo.Projects_GetById @ProjectId", new { ProjectId = id });
				return project;
			}
		}

		public Project Update(Project projectChanges)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var projectId = connection.Execute("dbo.Projects_Update", new
				{
					ProjectId = projectChanges.ProjectId,
					Name = projectChanges.Name,
					Description = projectChanges.Description,
					CreationTime = projectChanges.CreationTime,
					LastUpdateTime = projectChanges.LastUpdateTime,
					Hidden = projectChanges.Hidden
				}, commandType: CommandType.StoredProcedure);
				var project = GetById(projectChanges.ProjectId);
				return project;
			}
		}
	}
}
