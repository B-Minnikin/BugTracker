using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Database.Context;
using BugTracker.Models;
using BugTracker.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Repository.EFCoreRepositories;

public class EfBugReportRepository : IBugReportRepository
{
    private readonly BugReportContext context;
    private readonly ProjectContext projectContext;

    public EfBugReportRepository(BugReportContext context,
        ProjectContext projectContext)
    {
        this.context = context;
        this.projectContext = projectContext;
    }
    
    public async Task<BugReport> Add(BugReport model)
    {
        context.Add(model);
        await context.SaveChangesAsync();

        return model;
    }

    public async Task<BugReport> Update(BugReport model)
    {
        context.BugReports.Update(model);
        await context.SaveChangesAsync();

        return model;
    }

    public async Task<BugReport> Delete(int id)
    {
        var report = await context.BugReports.FirstOrDefaultAsync(br => br.BugReportId == id);
        report.Hidden = true;

        context.BugReports.Update(report);
        await context.SaveChangesAsync();

        return report;
    }

    public async Task<BugReport> GetById(int id)
    {
        var report = await context.BugReports.FirstOrDefaultAsync(br => br.BugReportId == id);

        return report;
    }

    public async Task<IEnumerable<BugReport>> GetAllById(int projectId)
    {
        // project id
        var reports = await context.BugReports.Where(br => br.ProjectId == projectId).ToListAsync();
        // TODO - check that this works
        return reports;
    }

    public async Task<BugReport> GetBugReportByLocalId(int localBugReportId, int projectId)
    {
        var report = await context.BugReports.FirstOrDefaultAsync(br =>
            br.LocalBugReportId == localBugReportId && br.ProjectId == projectId);
        return report;
    }

    public int GetCommentCountById(int bugReportId)
    {
        var count = context.Comments.Count(br => br.BugReportId == bugReportId);
        return count;
    }

    public async Task AddLocalBugReportId(int projectId)
    {
        var projectBugReportId = new ProjectBugReportId
        {
            ProjectId = projectId,
            NextFreeId = 1
        };

        projectContext.ProjectBugReportIds.Add(projectBugReportId);
        await projectContext.SaveChangesAsync();
    }

    public async Task AddUserAssignedToBugReport(int userId, int bugReportId)
    {
        var userBugReport = new UserBugReport
        {
            BugReportId = bugReportId,
            UserId = userId
        };

        context.UserBugReports.Add(userBugReport);
        await context.SaveChangesAsync();
    }

    public async Task DeleteUserAssignedToBugReport(int userId, int bugReportId)
    {
        var userBugReport =
            await context.UserBugReports.FirstOrDefaultAsync(ub =>
                ub.UserId == userId && ub.BugReportId == bugReportId);
        if (userBugReport == null)
        {
            // log error
            // throw error
            return;
        }

        context.UserBugReports.Remove(userBugReport);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<BugReport>> GetBugReportsForAssignedUser(int userId)
    {
        var reports = await context.UserBugReports
            .Where(ub => ub.UserId == userId)
            .Include(ub => ub.BugReport)
            .Select(ub => ub.BugReport)
            .ToListAsync();

        return reports;
    }

    public async Task<IEnumerable<ApplicationUser>> GetAssignedUsersForBugReport(int bugReportId)
    {
        var users = await context.UserBugReports
            .Where(ub => ub.BugReportId == bugReportId)
            .Include(ub => ub.User)
            .Select(ub => ub.User)
            .ToListAsync();

        return users;
    }

    public async Task AddBugReportLink(int bugReportId, int linkToBugReportId)
    {
        var bugReportLink = new BugReportLink
        {
            BugReportId = bugReportId,
            LinkedBugReportId = linkToBugReportId
        };
        context.BugReportLinks.Add(bugReportLink);
        await context.SaveChangesAsync();
    }

    public async Task DeleteBugReportLink(int bugReportId, int linkToBugReportId)
    {
        var bugReportLink = await context.BugReportLinks.FirstOrDefaultAsync(bl =>
            bl.BugReportId == bugReportId && bl.LinkedBugReportId == linkToBugReportId);

        context.BugReportLinks.Remove(bugReportLink);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<BugReport>> GetLinkedReports(int bugReportId)
    {
        var linkedReports = await context.BugReportLinks.Where(bl => bl.BugReportId == bugReportId)
            .Include(bl => bl.BugReport).Select(bl => bl.BugReport).ToListAsync();

        return linkedReports;
    }

    public Task<IEnumerable<AttachmentPath>> GetAttachmentPaths(AttachmentParentType parentType, int parentId)
    {
        throw new System.NotImplementedException();
    }
}
