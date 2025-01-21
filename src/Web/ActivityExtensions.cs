using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Web;

public static class ActivityExtensions
{
    public const string ActivitySourceName = "Web";

    public static Activity StartNewChildActivity([CallerMemberName] string name = "") =>
        new ActivitySource(ActivitySourceName).StartActivity(name) ?? new Activity(name).Start();
}