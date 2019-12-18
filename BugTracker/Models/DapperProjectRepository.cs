using Dapper;
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
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection("server=(localdb)\\MSSQLLocalDB;database=BugTrackerDB_cookies;Trusted_Connection=true")) // rather than static helper class function - string here
			{
				//var query = connection.Query<Project>("Select * FROM Projects");
				var result = connection.Execute("dbo.Projects_Insert @Name @Description @CreationTime @LastUpdateTime @Hidden",
					new { Name = project.Name, Description = project.Description, CreationTime = project.CreationTime, LastUpdateTime = project.LastUpdateTime, Hidden = project.Hidden });
				var query = connection.QueryFirst<Project>("dbo.Projects_GetProject @ProjectId", new { ProjectId = result });
				return query;
			}
		}

		public Project Delete(int Id)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection("server=(localdb)\\MSSQLLocalDB;database=BugTrackerDB_cookies;Trusted_Connection=true"))
			{
				var deletedProject = connection.QueryFirst("dbo.Projects_GetById @ProjectId", new { ProjectId = Id });
				var result = connection.Execute("dbo.Projects_Delete @ProjectId", new { ProjectId = Id });

				return deletedProject;
			}
		}

		public IEnumerable<Project> GetAllProjects()
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection("server=(localdb)\\MSSQLLocalDB;database=BugTrackerDB_cookies;Trusted_Connection=true"))
			{
				var query = connection.Query<Project>("dbo.Projects_GetAll");
				return query;
			}
		}

		public Project GetProject(int Id)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection("server=(localdb)\\MSSQLLocalDB;database=BugTrackerDB_cookies;Trusted_Connection=true")) // rather than static helper class function - string here
			{
				var output = connection.QueryFirst<Project>("dbo.Projects_GetById @ProjectId", new { ProjectId = Id });
				return output;
			}
		}

		public Project Update(Project projectChanges)
		{
			throw new NotImplementedException();
		}
	}
}
