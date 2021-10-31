using BugTracker.Models;
using BugTracker.Repository.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BugTracker.Repository.Interfaces
{
	public interface IMilestoneRepository : IAdd<Milestone>,
		IUpdate<Milestone>, IDelete<Milestone>,
		IGetById<Milestone>, IGetAllById<Milestone>
	{
		Task AddMilestoneBugReport(int milestoneId, int bugReportId);
		Task RemoveMilestoneBugReport(int milestoneId, int bugReportId);
		Task<IEnumerable<MilestoneBugReportEntry>> GetMilestoneBugReportEntries(int milestoneId);
		Task<IEnumerable<BugReport>> GetMilestoneBugReports(int milestoneId);
	}
}
