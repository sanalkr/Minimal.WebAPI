using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Minimal.WebAPI.Models;
using Minimal.WebAPI.Tests.Helpers;

namespace Minimal.WebAPI.Tests
{
    public class BlogPostTests
    {
        [Fact]
        public async void GetPostReturnsNotFoundIfNotExists()
        {
            await using var context = new MockDb().CreateDbContext();

            var result = await BlogPostEndpoints.GetPost(1, context);

            Assert.IsType<Results<Ok<BlogPost>, NotFound>>(result);

            var notFoundResult = (NotFound)result.Result;

            Assert.NotNull(notFoundResult);
        }

        [Fact]
        public async void GetPostReturnsPostIfFound()
        {
            await using var context = new MockDb().CreateDbContext();

            context.BlogPosts.Add(new BlogPost
            {
                Id = 1,
                Title = "Test title",
                Content = "Test Content",
                IsPublished = false,
                CreatedOnUtc = DateTime.Now,
                UpdatedOnUtc = DateTime.Now,
                PublishedOnUtc = null
            });

            await context.SaveChangesAsync();


            var result = await BlogPostEndpoints.GetPost(1, context);

            Assert.IsType<Results<Ok<BlogPost>, NotFound>>(result);

            var okResult = (Ok<BlogPost>)result.Result;

            Assert.NotNull(okResult);

        }

        [Fact]
        public async void CreatePostReturnsNewPost()
        {
            await using var context = new MockDb().CreateDbContext();

            var post = new BlogPost
            {
                Title = "Test title",
                Content = "Test Content"
            };

            Created<BlogPost> result = await BlogPostEndpoints.CreatePost(post, context);

            Assert.NotNull(result);
            Assert.Equal(post.Title, result.Value.Title);
            Assert.NotEqual(0, result.Value.Id);
        }
    }
}