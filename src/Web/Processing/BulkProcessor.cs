using System.Diagnostics;

namespace Web.Processing;

public class BulkProcessor(IEnumerable<IProcessor> processors, ILogger<BulkProcessor> logger) : IBulkProcessor
{
    public async Task ProcessAllAsync(bool useV2)
    {
        logger.LogInformation("Bulk processor is processing ({UseV2})", useV2);
        foreach (IProcessor processor in processors)
        {
            var processorName = processor.GetType().Name;

            if (useV2)
            {
                await ActivityExtensions.StartNewRootActivityWithLinkToCurrentActivityV2Async(processorName, async () =>
                {
                    logger.LogInformation("Processing {ProcessorName}", processorName);
                    await processor.ProcessAsync();
                });
            }
            else
            {
                using Activity batchActivity = ActivityExtensions.StartNewRootActivityWithLinkToCurrentActivity(processorName);
                logger.LogInformation("Processing {ProcessorName}", processorName);
                await processor.ProcessAsync();
            }
        }
    }
}