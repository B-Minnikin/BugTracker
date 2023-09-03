using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Database.Context;
using BugTracker.Models;
using BugTracker.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Repository.EFCoreRepositories;

public class EfSearchRepository : ISearchRepository
{
    private readonly ApplicationContext context;

    public EfSearchRepository(ApplicationContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<UserTypeaheadSearchResult>> GetMatchingProjectMembersBySearchQuery(string query, int projectId)
    {
        var results = await context.UserRoles.Where(ur => ur.ProjectId == projectId)
            .Join(context.Users,
                ur => ur.UserId,
                u => u.Id,
                (ur, u) => new { UserRole = ur, User = u })
            .Where(j => j.User.NormalizedUserName.Contains(query.ToUpper()))
            .Select(j => new UserTypeaheadSearchResult
            {
                UserName = j.User.UserName,
                Email = j.User.Email
            }).ToListAsync();

        return results;
    }

    public async Task<IEnumerable<BugReportTypeaheadSearchResult>> GetMatchingBugReportsByLocalIdSearchQuery(int localBugReportId, int projectId)
    {
        var bugReportSearchResults =
            await context.BugReports
                .Where(br => br.LocalBugReportId == localBugReportId && br.ProjectId == projectId)
                .Select(br => new BugReportTypeaheadSearchResult
                {
                    BugReportId = br.BugReportId,
                    LocalBugReportId = br.LocalBugReportId,
                    Title = br.Title
                })
                .ToListAsync();

        return bugReportSearchResults;
    }

    public async Task<IEnumerable<BugReportTypeaheadSearchResult>> GetMatchingBugReportsByTitleSearchQuery(string query, int projectId)
    {
        var bugReportSearchResults =
            await context.BugReports
                .Where(br => br.ProjectId == projectId && br.Title.Contains(query))
                .Select(br => new BugReportTypeaheadSearchResult
                {
                    BugReportId = br.BugReportId,
                    LocalBugReportId = br.LocalBugReportId,
                    Title = br.Title
                })
                .ToListAsync();

        return bugReportSearchResults;
    }
}
