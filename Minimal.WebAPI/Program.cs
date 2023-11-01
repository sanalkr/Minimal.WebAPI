
using Microsoft.EntityFrameworkCore;
using Minimal.WebAPI;
using Minimal.WebAPI.Models;
using System.Reflection.Metadata;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BlogPostDbContext>(opt => opt.UseInMemoryDatabase("BlogPostDb"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

var blogpostsApi = app.MapGroup("/blogposts");

blogpostsApi.MapGet("/", async (BlogPostDbContext context) =>
        await context.BlogPosts.ToListAsync());

blogpostsApi.MapGet("/{id}", async (int id, BlogPostDbContext context) =>
        await context.BlogPosts.FindAsync(id)
            is BlogPost blog ? Results.Ok(blog) : Results.NotFound()
       );

blogpostsApi.MapPost("/", async (BlogPost blog, BlogPostDbContext context) =>
{
    blog.CreatedOnUtc = DateTime.UtcNow;
    blog.UpdatedOnUtc = DateTime.UtcNow;
    
    context.BlogPosts.Add(blog);
    await context.SaveChangesAsync();

    return Results.Created($"/blogposts/{blog.Id}", blog);
});

blogpostsApi.MapPut("/{id}", async (int id, BlogPost blog, BlogPostDbContext context) =>
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
});

blogpostsApi.MapPost("/publish/{id}", async (int id, BlogPostDbContext context) =>
{
    var existingBlog = await context.BlogPosts.FindAsync(id);

    if (existingBlog is null) return Results.NotFound();
    
    existingBlog.IsPublished = true;
    existingBlog.PublishedOnUtc = DateTime.UtcNow;
    existingBlog.UpdatedOnUtc = DateTime.UtcNow;

    await context.SaveChangesAsync();

    return Results.NoContent();
});

blogpostsApi.MapDelete("/{id}", async (int id, BlogPostDbContext context) =>
{
    if (await context.BlogPosts.FindAsync(id) is BlogPost todo)
    {
        context.BlogPosts.Remove(todo);
        await context.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});


app.Run();
