using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
	public class ActivityFeedViewModel
	{
		public List<BugTracker.Models.Activity> Activities { get; set; } = new List<BugTracker.Models.Activity>();
	}
}
