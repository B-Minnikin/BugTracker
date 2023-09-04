using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Database.Context;
using BugTracker.Models;
using BugTracker.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Repository.EFCoreRepositories;

public class EfUserSubscriptionsRepository : IUserSubscriptionsRepository
{
    private readonly ApplicationContext context;

    public EfUserSubscriptionsRepository(ApplicationContext context)
    {
        this.context = context;
    }
    
    public async Task AddSubscription(string userId, int bugReportId)
    {
        var userSubscription = new UserSubscription
        {
            BugReportId = bugReportId,
            UserId = userId
        };

        context.UserSubscriptions.Add(userSubscription);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<BugReport>> GetSubscribedReports(string userId)
    {
        var reports = await context.UserSubscriptions
            .Where(us => us.UserId == userId)
            .Include(us => us.BugReport)
            .Select(us => us.BugReport)
            .ToListAsync();

        return reports;
    }

    public async Task<bool> IsSubscribed(string userId, int bugReportId)
    {
        return await context.UserSubscriptions
            .AnyAsync(us => us.BugReportId == bugReportId && us.UserId == userId);
    }

    public async Task DeleteSubscription(string userId, int bugReportId)
    {
        var userSubscription =
            await context.UserSubscriptions.FirstOrDefaultAsync(us =>
                us.UserId == userId && us.BugReportId == bugReportId);
        context.Remove(userSubscription);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<string>> GetAllSubscribedUserIds(int bugReportId)
    {
        var userIds = await context.UserSubscriptions
            .Where(us => us.BugReportId == bugReportId)
            .Select(us => us.UserId)
            .ToListAsync();

        return userIds;
    }
}
