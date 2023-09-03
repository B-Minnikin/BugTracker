using System.ComponentModel.DataAnnotations;

namespace BugTracker.ViewModels
{
	public class InvitesViewModel
	{
		[Required]
		public int ProjectId { get; set; }

		[Required]
		[EmailAddress]
		public string EmailAddress { get; set; }
	}
}
