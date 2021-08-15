using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Services
{
	interface IActivityMessageBuilder
	{
		void GenerateMessages(IEnumerable<Activity> activities);
		string GetMessage(Activity activity);
	}
}
