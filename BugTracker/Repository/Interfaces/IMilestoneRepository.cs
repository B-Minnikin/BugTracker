using BugTracker.Models;
using BugTracker.Repository.Common;
using System.Collections.Generic;

namespace BugTracker.Repository.Interfaces
{
	public interface IMilestoneRepository : IAdd<Milestone>,
		IUpdate<Milestone>, IDelete<Milestone>,
		IGetById<Milestone>, IGetAllById<Milestone>
	{
		void AddMilestoneBugReport(int milestoneId, int bugReportId);
		void RemoveMilestoneBugReport(int milestoneId, int bugReportId);
		IEnumerable<MilestoneBugReportEntry> GetMilestoneBugReportEntries(int milestoneId);
		IEnumerable<BugReport> GetMilestoneBugReports(int milestoneId);
	}
}
