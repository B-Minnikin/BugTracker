using BugTracker.Models;
using BugTracker.Repository.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BugTracker.Repository.DapperRepositories;

public class DapperProjectRepository : DapperBaseRepository, IProjectRepository
{
	public DapperProjectRepository(string connectionString) : base(connectionString) { }

	public async Task<Project> Add(Project project)
	{
		using IDbConnection connection = GetConnectionString();
		var insertedProjectId = await connection.ExecuteScalarAsync("dbo.Projects_Insert", new {
				project.Name, project.Description, project.CreationTime,
				project.LastUpdateTime, project.Hidden },
			commandType: CommandType.StoredProcedure);
		var insertedProject = await connection.QueryFirstAsync<Project>("dbo.Projects_GetById @ProjectId", new { ProjectId = insertedProjectId });
		return insertedProject;
	}

	public async Task<Project> Delete(int id)
	{
		using IDbConnection connection = GetConnectionString();
		var deletedProject = await connection.QueryFirstAsync<Project>("dbo.Projects_GetById @ProjectId", new { ProjectId = id });
		_ = await connection.ExecuteAsync("dbo.Projects_Delete @ProjectId", new { ProjectId = id });

		return deletedProject;
	}

	public async Task<IEnumerable<Project>> GetAll()
	{
		using IDbConnection connection = GetConnectionString();
		var projects = await connection.QueryAsync<Project>("dbo.Projects_GetAll");
		return projects;
	}

	public async Task<Project> GetById(int id)
	{
		using IDbConnection connection = GetConnectionString();
		var project = await connection.QueryFirstAsync<Project>("dbo.Projects_GetById @ProjectId", new { ProjectId = id });
		return project;
	}

	public async Task<Project> Update(Project projectChanges)
	{
		using IDbConnection connection = GetConnectionString();
		_ = await connection.ExecuteAsync("dbo.Projects_Update", new
		{
			projectChanges.ProjectId,
			projectChanges.Name,
			projectChanges.Description,
			projectChanges.CreationTime,
			projectChanges.LastUpdateTime,
			projectChanges.Hidden
		}, commandType: CommandType.StoredProcedure);
		var project = await GetById(projectChanges.ProjectId);
		return project;
	}
}
