using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class Project
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime CreationTime { get; set; }
		public DateTime LastUpdateTime { get; set; }
		public bool Hidden { get; set; }

		public IEnumerable<BugReport> BugReports { get; set; }
	}
}
