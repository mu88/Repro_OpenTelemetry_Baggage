using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Web;

public static class ActivityExtensions
{
    public const string ActivitySourceName = "Web";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName, "1.2.3.4");

    public static Activity StartNewChildActivity([CallerMemberName] string name = "") => ActivitySource.StartActivity(name) ?? new Activity(name).Start();
}