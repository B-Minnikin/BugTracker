using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.ViewModels
{
	public class AssignMemberViewModel
	{
		public int ProjectId { get; set; }
		public int BugReportId { get; set; }
		public List<string> MemberEmails { get; set; } = new List<string>();
	}
}
