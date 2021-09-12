using BugTracker.Models;
using BugTracker.Repository.Common;

namespace BugTracker.Repository.Interfaces
{
	public interface IBugReportStatesRepository : IAdd<BugState>,
		IGetById<BugState>, IGetAllById<BugState>
	{
		BugState GetLatestState(int bugReportId);
	}
}
