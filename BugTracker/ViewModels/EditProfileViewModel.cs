using System.Collections.Generic;
using BugTracker.Models;

namespace BugTracker.ViewModels
{
	public class EditProfileViewModel
	{
		public ApplicationUser User { get; set; }
		public List<Activity> Activities { get; set; }
	}
}
