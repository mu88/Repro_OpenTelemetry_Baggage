namespace Web;

public interface IBlogService
{
    Task<IEnumerable<Blog>> GetBlogsAsync();
}