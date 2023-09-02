using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Database.Context;
using BugTracker.Models;
using BugTracker.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Repository.EFCoreRepositories;

public class EfSearchRepository : EFCoreBaseRepository, ISearchRepository
{
    private readonly ApplicationContext context;

    public EfSearchRepository(ApplicationContext context)
    {
        this.context = context;
    }

    public Task<IEnumerable<UserTypeaheadSearchResult>> GetMatchingProjectMembersBySearchQuery(string query, int projectId)
    {
        // var users = userContext.Users.Where(u => u.)
        //
        // var searchResult = new UserTypeaheadSearchResult
        // {
        //     UserName = 
        // }
        throw new System.NotImplementedException();
    }

    public Task<IEnumerable<BugReportTypeaheadSearchResult>> GetMatchingBugReportsByLocalIdSearchQuery(int localBugReportId, int projectId)
    {
        throw new System.NotImplementedException();
    }

    public Task<IEnumerable<BugReportTypeaheadSearchResult>> GetMatchingBugReportsByTitleSearchQuery(string query, int projectId)
    {
        throw new System.NotImplementedException();
    }
}
