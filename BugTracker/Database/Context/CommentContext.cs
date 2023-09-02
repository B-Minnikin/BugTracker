using BugTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Database.Context;

public class CommentContext : DbContext
{
    public CommentContext(DbContextOptions contextOptions) : base(contextOptions)
    {
        
    }
    
    public DbSet<Comment> Comments { get; set; }
}
