
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Minimal.WebAPI;
using Minimal.WebAPI.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BlogPostDbContext>(opt => opt.UseInMemoryDatabase("BlogPostDb"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/hello", () => new BlogPost { Title = "New Title"});

var blogpostsApi = app.MapGroup("/blogposts")
    .MapBlogPostEndpoints()
    .WithTags("Blog Post APIs");




app.Run();
