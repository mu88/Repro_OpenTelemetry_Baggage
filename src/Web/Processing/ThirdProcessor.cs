namespace Web.Processing;

public class ThirdProcessor(ILogger<ThirdProcessor> logger) : IProcessor
{
    public async Task ProcessAsync() => await DoSomeWorkWithinThirdProcessorAsync();

    private async Task DoSomeWorkWithinThirdProcessorAsync()
    {
        ActivityExtensions.StartNewChildActivity();
        logger.LogInformation("Hello from ThirdProcessor");
        await Task.Delay(500);
    }
}