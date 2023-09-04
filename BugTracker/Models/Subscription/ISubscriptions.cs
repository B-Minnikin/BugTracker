using System.Threading.Tasks;
using BugTracker.Services;

namespace BugTracker.Models.Subscription;

public interface ISubscriptions
{
	Task<bool> IsSubscribed(string userId, int bugReportId);
	Task CreateSubscriptionIfNotSubscribed(string userId, int bugReportId);
	Task DeleteSubscription(string userId, int bugReportId);

	Task NotifyBugReportStateChanged(BugState bugState, ApplicationLinkGenerator linkGenerator, int bugReportId);
	Task NotifyBugReportNewComment(Comment comment, string bugReportUrl);
}
