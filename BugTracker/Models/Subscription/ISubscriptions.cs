using System.Threading.Tasks;

namespace BugTracker.Models.Subscription;

public interface ISubscriptions
{
	Task<bool> IsSubscribed(string userId, int bugReportId);
	Task CreateSubscriptionIfNotSubscribed(string userId, int bugReportId);
	Task DeleteSubscription(string userId, int bugReportId);

	Task NotifyBugReportStateChanged(BugState bugState, string bugReportUrl);
	Task NotifyBugReportNewComment(Comment comment, string bugReportUrl);
}
