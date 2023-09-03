using BugTracker.Models;

namespace BugTracker.ViewModels
{
	public class CreateCommentViewModel
	{
		public bool Subscribe { get; set; }
		public Comment Comment { get; set; } = new();
	}
}
