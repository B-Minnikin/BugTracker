using BugTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BugTracker.Services
{
	interface IActivityMessageBuilder
	{
		Task GenerateMessages(IEnumerable<Activity> activities);
		Task<string> GetMessage(Activity activity);
	}
}
