using BugTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BugTracker.Repository.Interfaces
{
	public interface ISearchRepository
	{
		Task<IEnumerable<UserTypeaheadSearchResult>> GetMatchingProjectMembersBySearchQuery(string query, int projectId);
		Task<IEnumerable<BugReportTypeaheadSearchResult>> GetMatchingBugReportsByLocalIdSearchQuery(int localBugReportId, int projectId);
		Task<IEnumerable<BugReportTypeaheadSearchResult>> GetMatchingBugReportsByTitleSearchQuery(string query, int projectId);
	}
}
