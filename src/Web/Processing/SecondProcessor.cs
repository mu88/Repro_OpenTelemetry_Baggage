namespace Web.Processing;

public class SecondProcessor(ILogger<SecondProcessor> logger) : IProcessor
{
    public async Task ProcessAsync() => await DoSomeWorkWithinSecondProcessorAsync();

    private async Task DoSomeWorkWithinSecondProcessorAsync()
    {
        ActivityExtensions.StartNewChildActivity();
        logger.LogInformation("Hello from SecondProcessor");
        await Task.Delay(500);
    }
}