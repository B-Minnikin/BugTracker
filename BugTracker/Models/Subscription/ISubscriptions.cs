using BugTracker.Services;
using System.Threading.Tasks;

namespace BugTracker.Models.Subscription;

public interface ISubscriptions
{
    Task<bool> IsSubscribed(int userId, int bugReportId);
    Task CreateSubscriptionIfNotSubscribed(int userId, int bugReportId);
    Task DeleteSubscription(int userId, int bugReportId);

    Task NotifyBugReportStateChanged(BugState bugState, ApplicationLinkGenerator linkGenerator, int bugReportId);
    Task NotifyBugReportNewComment(Comment comment, string bugReportUrl);
}
