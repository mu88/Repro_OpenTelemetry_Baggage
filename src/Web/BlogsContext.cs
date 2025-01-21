using Microsoft.EntityFrameworkCore;

namespace Web;

public class BlogsContext(DbContextOptions<BlogsContext> options) : DbContext(options)
{
    public DbSet<Blog> Blogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.Entity<Blog>().HasKey(blog => blog.Key);
}