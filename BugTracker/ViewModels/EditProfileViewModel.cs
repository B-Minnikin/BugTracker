using BugTracker.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
	public class EditProfileViewModel
	{
		public IdentityUser User { get; set; }
		public List<BugTracker.Models.Activity> Activities { get; set; }
	}
}
