
namespace BugTracker.Models.ProjectInvitation
{
	public class ProjectInvitation
	{
		public string EmailAddress { get; set; }
		public Project Project { get; set; }
		public ApplicationUser ToUser { get; set; }
		public ApplicationUser FromUser { get; set; }
	}
}
