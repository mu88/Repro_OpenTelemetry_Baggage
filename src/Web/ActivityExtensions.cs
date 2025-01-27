using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Web;

public static class ActivityExtensions
{
    public const string ActivitySourceName = "Web";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName, "1.2.3.4");

    public static Activity StartNewChildActivity([CallerMemberName] string name = "") => ActivitySource.StartActivity(name) ?? new Activity(name).Start();

    public static Activity StartNewRootActivityWithLinkToCurrentActivity(string name)
    {
        Activity? newRootActivity = null;
        Activity? previousActivity = Activity.Current;
        Activity.Current = null;
        if (previousActivity != null)
        {
            newRootActivity = ActivitySource.StartActivity(name);
            if (newRootActivity != null)
            {
                newRootActivity.AddLink(new ActivityLink(previousActivity.Context));
                previousActivity.AddLink(new ActivityLink(newRootActivity.Context));
            }
        }

        return newRootActivity ?? new Activity(name).Start();
    }

    public static async Task StartNewRootActivityWithLinkToCurrentActivityV2Async(string name, Func<Task> runAsync)
    {
        Activity? newRootActivity = null;
        Activity? previousActivity = Activity.Current;
        Activity.Current = null;
        if (previousActivity != null)
        {
            newRootActivity = ActivitySource.StartActivity(name);
            if (newRootActivity != null)
            {
                newRootActivity.AddLink(new ActivityLink(previousActivity.Context));
                previousActivity.AddLink(new ActivityLink(newRootActivity.Context));
            }
        }

        try
        {
            await runAsync();
        }
        finally
        {
            newRootActivity?.Dispose();
            Activity.Current = previousActivity;
        }
    }
}