using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class BugState
	{
		[Key]
		public int Id { get; set; }
		public DateTime Time { get; set; }
		public string Author { get; set; } // link to profile
		public string StateName { get; set; }
	}
}
