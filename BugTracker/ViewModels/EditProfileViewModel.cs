﻿using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using BugTracker.Models;

namespace BugTracker.ViewModels
{
	public class EditProfileViewModel
	{
		public IdentityUser User { get; set; }
		public List<Activity> Activities { get; set; }
	}
}
