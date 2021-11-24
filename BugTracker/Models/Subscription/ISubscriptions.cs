using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Database
{
	public interface ISubscriptions
	{
		Task<bool> IsSubscribed(int userId, int bugReportId);
		Task CreateSubscriptionIfNotSubscribed(int userId, int bugReportId);
		Task DeleteSubscription(int userId, int bugReportId);

		Task NotifyBugReportStateChanged(BugState bugState, string bugReportUrl);
		Task NotifyBugReportNewComment(Comment comment, string bugReportUrl);
	}
}
