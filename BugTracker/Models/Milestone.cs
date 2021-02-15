using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class Milestone
	{
		public int MilestoneId { get; set; }
		public int ProjectId { get; set; }
		[Required(ErrorMessage = "Please enter a title")]
		public string Title { get; set; }
		public string Description { get; set; }
		public DateTime CreationTime { get; set; } = DateTime.Now;
		public DateTime DueDate { get; set; } = DateTime.Now.AddDays(7);
	}
}
