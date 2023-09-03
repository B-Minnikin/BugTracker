using System;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
	public enum StateType
	{
		Open,
		Resolved,
		Closed
	}

	public class BugState
	{
		[Key]
		public int BugStateId { get; set; }
		public DateTime Time { get; set; }
		public StateType StateType { get; set; }
		public string Author { get; set; } // link to profile
		public int BugReportId { get; set; }
	}
}
