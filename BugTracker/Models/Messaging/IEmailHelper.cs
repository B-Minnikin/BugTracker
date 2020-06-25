using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
	public interface IEmailHelper
	{
		void Send(string userName, string emailAddress, string subject, string messageBody);
	}
}
