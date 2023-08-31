using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class Comment
	{
		[Key]
		public int CommentId { get; set; }
		public int AuthorId { get; set; }
		public DateTime Date { get; set; }
		public string MainText { get; set; }
		public int BugReportId { get; set; }
		public bool Hidden { get; set; }
	}
}
