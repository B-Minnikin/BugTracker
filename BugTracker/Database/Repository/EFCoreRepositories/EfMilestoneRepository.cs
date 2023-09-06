using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Database.Context;
using BugTracker.Database.Repository.Interfaces;
using BugTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Database.Repository.EFCoreRepositories;

public class EfMilestoneRepository : IMilestoneRepository
{
    private readonly ApplicationContext context;

    public EfMilestoneRepository(ApplicationContext context)
    {
        this.context = context;
    }

    public async Task<Milestone> Add(Milestone milestone)
    {
        context.Milestones.Add(milestone);
        await context.SaveChangesAsync();

        return milestone;
    }

    public async Task<Milestone> Update(Milestone milestone)
    {
        context.Milestones.Update(milestone);
        await context.SaveChangesAsync();

        return milestone;
    }

    public async Task<Milestone> Delete(int milestoneId)
    {
        var milestone = await context.Milestones.FindAsync(milestoneId);
        if (milestone is null) throw new NullReferenceException("Requested milestone was not found and returned null");
        
        milestone.Hidden = true;

        context.Milestones.Update(milestone);
        await context.SaveChangesAsync();

        return milestone;
    }

    public async Task<Milestone> GetById(int milestoneId)
    {
        var milestone = await context.Milestones.FindAsync(milestoneId);
        return milestone;
    }

    public async Task<IEnumerable<Milestone>> GetAllById(int projectId)
    {
        var milestones = await context.Milestones
            .Where(m => m.ProjectId == projectId)
            .ToListAsync();

        return milestones;
    }

    public async Task AddMilestoneBugReport(int milestoneId, int bugReportId)
    {
        var milestoneBugReport = new MilestoneBugReport
        {
            MilestoneId = milestoneId,
            BugReportId = bugReportId
        };
        context.MilestoneBugReports.Add(milestoneBugReport);
        await context.SaveChangesAsync();
    }

    public async Task RemoveMilestoneBugReport(int milestoneId, int bugReportId)
    {
        var milestoneBugReport = await context.MilestoneBugReports
            .FirstOrDefaultAsync(mbr =>
                mbr.BugReportId == bugReportId && mbr.MilestoneId == milestoneId);

        context.MilestoneBugReports.Remove(milestoneBugReport);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<MilestoneBugReportEntry>> GetMilestoneBugReportEntries(int milestoneId)
    {
        var entries = await context.MilestoneBugReports
            .Where(mbr => mbr.MilestoneId == milestoneId)
            .Select(mbr => new MilestoneBugReportEntry
            {
                BugReportId = mbr.BugReport.BugReportId,
                LocalBugReportId = mbr.BugReport.LocalBugReportId,
                Title = mbr.BugReport.Title
            })
            .ToListAsync();

        return entries;
    }

    public async Task<IEnumerable<BugReport>> GetMilestoneBugReports(int milestoneId)
    {
        var bugReports = await context.MilestoneBugReports
            .Where(mbr => mbr.MilestoneId == milestoneId)
            .Select(mbr => mbr.BugReport)
            .ToListAsync();

        return bugReports;
    }
}
