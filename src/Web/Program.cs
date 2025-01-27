using System.Diagnostics;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using Web;
using Web.Processing;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

var connection = new SqliteConnection("Filename=:memory:");
await connection.OpenAsync();

builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddDbContext<BlogsContext>(options => options.UseSqlite(connection));
builder.Services.AddScoped<IBulkProcessor, BulkProcessor>();
builder.Services.AddScoped<IProcessor, FirstProcessor>();
builder.Services.AddScoped<IProcessor, SecondProcessor>();
builder.Services.AddScoped<IProcessor, ThirdProcessor>();
builder.ConfigureOpenTelemetry("Web");

WebApplication app = builder.Build();

app.MapGet("/seed", async (BlogsContext blogsContext) =>
{
    await blogsContext.Database.EnsureDeletedAsync();
    await blogsContext.Database.EnsureCreatedAsync();
    blogsContext.Blogs.AddRange(
        new Blog(Guid.NewGuid(), "First blog", "Bla"),
        new Blog(Guid.NewGuid(), "Second blog", "Bla bla"));
    await blogsContext.SaveChangesAsync();
});

app.MapGet("/blogs", async (IBlogService blogService) =>
{
    using Activity activity = ActivityExtensions.StartNewChildActivity();
    activity.SetTag("location", "web API handler");
    var identifier = Guid.NewGuid().ToString();
    activity.SetTag("custom_identifier_tag_Activity", identifier);
    Baggage.Current.SetBaggage("custom_identifier_baggage_OTEL", identifier);
    activity.SetBaggage("custom_identifier_baggage_Activity", identifier);
    try
    {
        return await blogService.GetBlogsAsync();
    }
    catch (Exception e)
    {
        activity.SetStatus(ActivityStatusCode.Error, e.ToString());
        throw;
    }
});

app.MapGet("/bulkProcessing", async (IBulkProcessor bulkProcessor) =>
{
    using Activity activity = ActivityExtensions.StartNewChildActivity("Within Web API");
    await bulkProcessor.ProcessAllAsync(false);
});

app.MapGet("/bulkProcessing2", async (IBulkProcessor bulkProcessor) =>
{
    using Activity activity = ActivityExtensions.StartNewChildActivity("Within Web API v2");
    await bulkProcessor.ProcessAllAsync(true);
});

app.Run();