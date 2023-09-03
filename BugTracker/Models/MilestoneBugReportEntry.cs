
namespace BugTracker.Models
{
	public class MilestoneBugReportEntry
	{
		public int BugReportId { get; set; }
		public BugState CurrentState { get; set; }
		public int LocalBugReportId { get; set; }
		public string Title { get; set; }
		public string Url { get; set; }
	}
}
