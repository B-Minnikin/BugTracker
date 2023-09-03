using BugTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BugTracker.Repository.Interfaces
{
	public interface IUserSubscriptionsRepository
	{
		Task AddSubscription(string userId, int bugReportId);
		Task<IEnumerable<BugReport>> GetSubscribedReports(string userId);
		Task<bool> IsSubscribed(string userId, int bugReportId);
		Task DeleteSubscription(string userId, int bugReportId);
		Task<IEnumerable<int>> GetAllSubscribedUserIds(int bugReportId);
	}
}
