using BugTracker.Models;
using BugTracker.Repository.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BugTracker.Repository.DapperRepositories
{
	public class DapperSearchRepository : DapperBaseRepository, ISearchRepository
	{
		public DapperSearchRepository(string connectionString) : base(connectionString) { }

		public async Task<IEnumerable<UserTypeaheadSearchResult>> GetMatchingProjectMembersBySearchQuery(string query, int projectId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var searchResults = await connection.QueryAsync<UserTypeaheadSearchResult>("dbo.Users_MatchByQueryAndProject", new { Query = query, ProjectId = projectId },
					commandType: CommandType.StoredProcedure);
				return searchResults;
			}
		}

		public async Task<IEnumerable<BugReportTypeaheadSearchResult>> GetMatchingBugReportsByTitleSearchQuery(string query, int projectId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var searchResults = await connection.QueryAsync<BugReportTypeaheadSearchResult>("dbo.BugReports_MatchByTitleQueryAndProject", new { Query = query, ProjectId = projectId },
					commandType: CommandType.StoredProcedure);
				return searchResults;
			}
		}

		public async Task<IEnumerable<BugReportTypeaheadSearchResult>> GetMatchingBugReportsByLocalIdSearchQuery(int localBugReportId, int projectId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var searchResults = await connection.QueryAsync<BugReportTypeaheadSearchResult>("dbo.BugReports_MatchByLocalIdAndProject", new { Query = localBugReportId, ProjectId = projectId },
					commandType: CommandType.StoredProcedure);
				return searchResults;
			}
		}
	}
}
