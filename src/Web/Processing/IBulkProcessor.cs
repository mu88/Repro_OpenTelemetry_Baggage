namespace Web.Processing;

public interface IBulkProcessor
{
    Task ProcessAllAsync(bool useV2);
}