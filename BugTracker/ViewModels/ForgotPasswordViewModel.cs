using System.ComponentModel.DataAnnotations;

namespace BugTracker.ViewModels
{
	public class ForgotPasswordViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}
}
