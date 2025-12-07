using System;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models;

public class Comment
{
	[Key]
	public int CommentId { get; set; }
	public int AuthorId { get; set; }
	public DateTime Date { get; set; }
	public string MainText { get; set; }
	public int BugReportId { get; set; }
}
