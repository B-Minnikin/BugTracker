using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
	public class NewMilestoneViewModel
	{
		public int ProjectId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public DateTime CreationTime { get; set; } = DateTime.Now;
		public DateTime DueDate { get; set; } = DateTime.Now.AddDays(7);

		public List<MilestoneBugReportEntry> MilestoneBugReportEntries { get; set; } = new List<MilestoneBugReportEntry>();
	}
}
