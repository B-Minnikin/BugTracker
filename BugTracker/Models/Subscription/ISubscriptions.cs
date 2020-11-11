using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Database
{
	public interface ISubscriptions
	{
		bool IsSubscribed(int userId, int bugReportId);
		void CreateSubscriptionIfNotSubscribed(int userId, int bugReportId);
		void DeleteSubscription(int userId, int bugReportId);

		Task NotifyBugReportStateChanged(BugState bugState, string bugReportUrl);
		Task NotifyBugReportNewComment(BugReportComment bugReportComment, string bugReportUrl);
	}
}
