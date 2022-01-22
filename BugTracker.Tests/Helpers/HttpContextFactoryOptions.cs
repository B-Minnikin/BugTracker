using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Tests.Helpers
{
	public class HttpContextFactoryOptions
	{
		public int ProjectId { get; set; } = 1;
		public string UserId { get; set; } = "1";
		public string UserName { get; set; } = "Test User";
	}
}
