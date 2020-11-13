using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
