using BugTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BugTracker.Repository.Interfaces
{
	public interface IUserSubscriptionsRepository
	{
		Task AddSubscription(int userId, int bugReportId);
		Task<IEnumerable<BugReport>> GetSubscribedReports(int userId);
		Task<bool> IsSubscribed(int userId, int bugReportId);
		Task DeleteSubscription(int userId, int bugReportId);
		Task<IEnumerable<int>> GetAllSubscribedUserIds(int bugReportId);
	}
}
