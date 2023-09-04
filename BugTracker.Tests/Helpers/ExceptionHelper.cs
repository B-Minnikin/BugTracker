
namespace BugTracker.Tests.Helpers
{
	class ExceptionHelper
	{
		public static void NotImplemented()
		{
			throw new Xunit.Sdk.XunitException("Not implemented");
		}
	}
}
