using Microsoft.EntityFrameworkCore;

namespace BugTracker.Tests.Helpers;

public static class EfContextHelper
{
    public static void EnsureInMemoryDeleted(this DbContext dbContext)
    {
        if (dbContext.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
        {
            dbContext.Database.EnsureDeleted();
        }
    }
}
