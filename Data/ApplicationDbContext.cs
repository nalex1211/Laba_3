using Microsoft.EntityFrameworkCore;
using Laba_3.Models;

namespace Laba_3.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Discussion> Discussions { get; set; }
    public DbSet<Comment> Comments { get; set; }
}
