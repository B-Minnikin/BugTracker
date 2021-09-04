using BugTracker.Models;
using BugTracker.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
