using BugTracker.Models;
using System.Collections.Generic;

namespace BugTracker.Repository.Interfaces
{
	public interface ISearchRepository
	{
		IEnumerable<UserTypeaheadSearchResult> GetMatchingProjectMembersBySearchQuery(string query, int projectId);
		IEnumerable<BugReportTypeaheadSearchResult> GetMatchingBugReportsByLocalIdSearchQuery(int localBugReportId, int projectId);
		IEnumerable<BugReportTypeaheadSearchResult> GetMatchingBugReportsByTitleSearchQuery(string query, int projectId);
	}
}
