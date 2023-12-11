using Microsoft.EntityFrameworkCore;

namespace Minimal.WebAPI.Tests.Helpers
{
    public class MockDb : IDbContextFactory<BlogPostDbContext>
    {
        public BlogPostDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<BlogPostDbContext>()
                .UseInMemoryDatabase($"BlogPostDb-{DateTime.Now.ToFileTimeUtc()}")
                .Options;

            return new BlogPostDbContext(options);
        }
    }
}
