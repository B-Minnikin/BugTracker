using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public class EmailHelper : IEmailHelper
	{
		public void Send(string userName, string emailAddress, string subject, string messageBody)
		{
			throw new NotImplementedException();
		}
	}
}
