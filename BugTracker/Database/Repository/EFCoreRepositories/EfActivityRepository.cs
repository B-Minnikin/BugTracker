using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Database.Context;
using BugTracker.Database.Repository.Interfaces;
using BugTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Database.Repository.EFCoreRepositories;

public class EfActivityRepository : IActivityRepository
{
    private readonly ApplicationContext context;

    public EfActivityRepository(ApplicationContext context)
    {
        this.context = context;
    }
    
    public async Task<Activity> Add(Activity activity)
    {
        context.Activities.Add(activity);
        await context.SaveChangesAsync();
        
        return activity;
    }

    public async Task<Activity> Delete(int activityId)
    {
        var activity = await context.Activities.FindAsync(activityId);
        if (activity is null) throw new NullReferenceException("Specified activity was null");
        
        activity.Hidden = true;

        context.Activities.Update(activity);
        await context.SaveChangesAsync();

        return activity;
    }

    public async Task<IEnumerable<Activity>> GetUserActivities(string userId)
    {
        var activities = await context.Activities
            .Where(a => a.UserId == userId)
            .ToListAsync();

        return activities;
    }

    public Task<IEnumerable<Activity>> GetBugReportActivities(int bugReportId)
    {
        // TODO - Work out how to resolve these
        throw new NotImplementedException();
    }
}
