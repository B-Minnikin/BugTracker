
namespace BugTracker.Models
{
	public class BugReportTypeaheadSearchResult
	{
		public int BugReportId { get; set; }
		public int LocalBugReportId { get; set; }
		public string Title { get; set; }
		public string Url { get; set; }
	}
}
