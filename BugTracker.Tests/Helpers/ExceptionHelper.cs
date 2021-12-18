using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
