using System.Collections.Generic;
using System.Threading.Tasks;
using BugTracker.Database.Repository.Common;
using BugTracker.Models;

namespace BugTracker.Database.Repository.Interfaces
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
