using BugTracker.Models;
using System.Collections.Generic;

namespace BugTracker.Repository.Interfaces
{
	public interface IUserSubscriptionsRepository
	{
		void AddSubscription(int userId, int bugReportId);
		IEnumerable<BugReport> GetSubscribedReports(int userId);
		bool IsSubscribed(int userId, int bugReportId);
		void DeleteSubscription(int userId, int bugReportId);
		IEnumerable<int> GetAllSubscribedUserIds(int bugReportId);
	}
}
