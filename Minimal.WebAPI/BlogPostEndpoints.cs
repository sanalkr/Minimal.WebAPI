using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Minimal.WebAPI.Models;

namespace Minimal.WebAPI
{
    public static class BlogPostEndpoints
    {
        public static RouteGroupBuilder MapBlogPostEndpoints(this RouteGroupBuilder blogpostsApiGroup)
        {
            blogpostsApiGroup.MapGet("/", GetAllBlogPosts).
                Produces<List<BlogPost>>();

            blogpostsApiGroup.MapGet("/{id}", GetPost);

            blogpostsApiGroup.MapPost("/", CreatePost);

            blogpostsApiGroup.MapPut("/{id}", UpdatePost);

            blogpostsApiGroup.MapPost("/publish/{id}", PublishPost);

            blogpostsApiGroup.MapDelete("/{id}", DeleteBlogPost);

            return blogpostsApiGroup;
        }

        public static async Task<IResult> GetAllBlogPosts(BlogPostDbContext context)
        {
            return Results.Ok(await context.BlogPosts.ToListAsync());
        }

        public static async Task<Results<Ok<BlogPost>, NotFound>> GetPost(int id, BlogPostDbContext context)
        {
            return await context.BlogPosts.FindAsync(id)
                                    is BlogPost blog ? TypedResults.Ok(blog) : TypedResults.NotFound();
        }

        public static async Task<Created<BlogPost>> CreatePost(BlogPost blog, BlogPostDbContext context)
        {
            blog.CreatedOnUtc = DateTime.UtcNow;
            blog.UpdatedOnUtc = DateTime.UtcNow;

            context.BlogPosts.Add(blog);
            await context.SaveChangesAsync();

            return TypedResults.Created($"/blogposts/{blog.Id}", blog);
        }

        public static async Task<IResult> UpdatePost(int id, BlogPost blog, BlogPostDbContext context)
        {
            var existingBlog = await context.BlogPosts.FindAsync(id);

            if (existingBlog is null) return Results.NotFound();

            existingBlog.Title = blog.Title;
            existingBlog.Content = blog.Content;
            existingBlog.UpdatedOnUtc = DateTime.UtcNow;

            if (blog.IsPublished && existingBlog.PublishedOnUtc == null)
            {
                existingBlog.IsPublished = blog.IsPublished;
                existingBlog.PublishedOnUtc = DateTime.UtcNow;
            }

            await context.SaveChangesAsync();

            return Results.NoContent();
        }

        public static async Task<IResult> PublishPost(int id, BlogPostDbContext context)
        {
            var existingBlog = await context.BlogPosts.FindAsync(id);

            if (existingBlog is null) return Results.NotFound();

            existingBlog.IsPublished = true;
            existingBlog.PublishedOnUtc = DateTime.UtcNow;
            existingBlog.UpdatedOnUtc = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return Results.NoContent();
        }

        public static async Task<IResult> DeleteBlogPost(int id, BlogPostDbContext context)
        {
            if (await context.BlogPosts.FindAsync(id) is BlogPost todo)
            {
                context.BlogPosts.Remove(todo);
                await context.SaveChangesAsync();
                return Results.NoContent();
            }

            return Results.NotFound();
        }
    }
}
