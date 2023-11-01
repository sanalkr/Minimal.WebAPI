using Microsoft.EntityFrameworkCore;
using Minimal.WebAPI.Models;

namespace Minimal.WebAPI
{
    public class BlogPostDbContext : DbContext
    {
        public BlogPostDbContext(DbContextOptions<BlogPostDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    }
}
