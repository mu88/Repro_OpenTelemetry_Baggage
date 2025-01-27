namespace Web.Processing;

public class FirstProcessor(ILogger<FirstProcessor> logger) : IProcessor
{
    public async Task ProcessAsync() => await DoSomeWorkWithinFirstProcessorAsync();

    private async Task DoSomeWorkWithinFirstProcessorAsync()
    {
        ActivityExtensions.StartNewChildActivity();
        logger.LogInformation("Hello from FirstProcessor");
        await Task.Delay(500);
    }
}