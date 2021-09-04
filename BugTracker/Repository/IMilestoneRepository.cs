using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Repository
{
	public interface IMilestoneRepository
	{
		Milestone AddMilestone(Milestone milestone);
		Milestone DeleteMilestone(int milestoneId);
		Milestone UpdateMilestone(Milestone milestone);
		IEnumerable<Milestone> GetAllMilestones(int projectId);
		Milestone GetMilestoneById(int id);
		void AddMilestoneBugReport(int milestoneId, int bugReportId);
		void RemoveMilestoneBugReport(int milestoneId, int bugReportId);
		IEnumerable<MilestoneBugReportEntry> GetMilestoneBugReportEntries(int milestoneId);
		IEnumerable<BugReport> GetMilestoneBugReports(int milestoneId);
	}
}
