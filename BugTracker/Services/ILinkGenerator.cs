using Microsoft.AspNetCore.Routing;

namespace BugTracker.Services
{
	public interface ILinkGenerator
	{
		string GetPathByAction(string action, string controller, object values);
		string GetUriByAction(string action, string controller, object values);
	}
}