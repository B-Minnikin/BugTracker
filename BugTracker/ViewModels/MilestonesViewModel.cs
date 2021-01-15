using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
	public class MilestonesViewModel
	{
		public int ProjectId { get; set; }
		public bool ShowNewButton { get; set; } = false;

		public List<Milestone> ProjectMilestones { get; set; }
	}
}
