using BugTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Database.Context;

public class CommentContext : DbContext
{
    public DbSet<Comment> Comments { get; set; }
}
