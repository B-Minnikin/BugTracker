using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class Milestone
	{
		public int MilestoneId { get; set; }
		public int ProjectId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public DateTime CreationTime { get; set; } = DateTime.Now;
		public DateTime DueDate { get; set; }
	}
}
