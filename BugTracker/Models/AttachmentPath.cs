using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
	public enum AttachmentParentType
	{
		BugReport, Comment
	}

	public class AttachmentPath
	{
		[Key]
		public int AttachmentPathId { get; set; }
		public string Path { get; set; }
	}
}
