using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class AttachmentPath
	{
		[Key]
		public int AttachmentPathId { get; set; }
		public string Path { get; set; }
	}
}
