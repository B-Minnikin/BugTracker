using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Helpers
{
	public class MilestoneBugReportEntryEqualityComparer : IEqualityComparer<MilestoneBugReportEntry>
	{
		public bool Equals([AllowNull] MilestoneBugReportEntry x, [AllowNull] MilestoneBugReportEntry y)
		{
			if (x == null && y == null)
				return true;
			else if (x == null || y == null)
				return false;
			else if (x.LocalBugReportId == y.LocalBugReportId)
				return true;
			else return false;
		}

		public int GetHashCode([DisallowNull] MilestoneBugReportEntry obj)
		{
			return obj.LocalBugReportId;
		}
	}
}
