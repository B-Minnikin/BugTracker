using BugTracker.Models;
using BugTracker.Repository.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BugTracker.Repository.DapperRepositories
{
	public class DapperProjectRepository : DapperBaseRepository, IProjectRepository
	{
		public DapperProjectRepository(string connectionString) : base(connectionString) { }

		public async Task<Project> Add(Project project)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var insertedProjectId = await connection.ExecuteScalarAsync("dbo.Projects_Insert", new {
					Name = project.Name, Description = project.Description, CreationTime = project.CreationTime,
					LastUpdateTime = project.LastUpdateTime, Hidden = project.Hidden },
					commandType: CommandType.StoredProcedure);
				var insertedProject = await connection.QueryFirstAsync<Project>("dbo.Projects_GetById @ProjectId", new { ProjectId = insertedProjectId });
				return insertedProject;
			}
		}

		public async Task<Project> Delete(int id)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var deletedProject = await connection.QueryFirstAsync<Project>("dbo.Projects_GetById @ProjectId", new { ProjectId = id });
				var result = await connection.ExecuteAsync("dbo.Projects_Delete @ProjectId", new { ProjectId = id });

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

		public async Task<Project> GetById(int id)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var project = await connection.QueryFirstAsync<Project>("dbo.Projects_GetById @ProjectId", new { ProjectId = id });
				return project;
			}
		}

		public async Task<Project> Update(Project projectChanges)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var projectId = await connection.ExecuteAsync("dbo.Projects_Update", new
				{
					ProjectId = projectChanges.ProjectId,
					Name = projectChanges.Name,
					Description = projectChanges.Description,
					CreationTime = projectChanges.CreationTime,
					LastUpdateTime = projectChanges.LastUpdateTime,
					Hidden = projectChanges.Hidden
				}, commandType: CommandType.StoredProcedure);
				var project = await GetById(projectChanges.ProjectId);
				return project;
			}
		}
	}
}
