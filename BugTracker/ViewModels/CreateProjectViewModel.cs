using System.ComponentModel.DataAnnotations;

namespace BugTracker.ViewModels
{
	public class CreateProjectViewModel
	{
		[Required]
		public string Name { get; set; }
		public string Description { get; set; }
	}
}
