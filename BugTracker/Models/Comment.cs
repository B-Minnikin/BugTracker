using System;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
	public class Comment
	{
		[Key]
		public int CommentId { get; set; }
		public string AuthorId { get; set; }
		public DateTime Date { get; set; }
		public string MainText { get; set; }
		public int BugReportId { get; set; }
		public bool Hidden { get; set; }
	}
}
