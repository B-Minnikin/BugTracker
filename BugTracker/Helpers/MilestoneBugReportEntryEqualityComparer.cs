using BugTracker.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace BugTracker.Helpers;

public class MilestoneBugReportEntryEqualityComparer : IEqualityComparer<MilestoneBugReportEntry>
{
    public bool Equals([AllowNull] MilestoneBugReportEntry x, [AllowNull] MilestoneBugReportEntry y)
    {
        return x switch
        {
            null when y == null => true,
            _ when x == null || y == null => false,
            _ when x.LocalBugReportId == y.LocalBugReportId => true,
            _ => false
        };
    }

    public int GetHashCode([DisallowNull] MilestoneBugReportEntry obj)
    {
        return obj.LocalBugReportId;
    }
}