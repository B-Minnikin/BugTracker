using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
	public class MilestoneContainer
	{
		public Milestone Milestone { get; set; }
		public MilestoneProgress MilestoneProgress { get; set; }
	}
}
