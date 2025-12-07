using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace BugTracker.ViewModels;

public class EditProfileViewModel
{
	public IdentityUser User { get; set; }
	public List<Models.Activity> Activities { get; set; }
}
