using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public enum StateType
	{
		open,
		resolved,
		closed
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
