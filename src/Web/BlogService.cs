using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Web;

public class BlogService(BlogsContext blogsContext) : IBlogService
{
    public async Task<IEnumerable<Blog>> GetBlogsAsync()
    {
        using Activity activity = ActivityExtensions.StartNewChildActivity();
        activity.SetTag("location", nameof(GetBlogsAsync));

        return await blogsContext.Blogs.ToListAsync();
    }
}