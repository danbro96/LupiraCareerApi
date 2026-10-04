using System.Diagnostics;

namespace LupiraCareerApi.Core.Domain.Shared;

/// <summary>Domain-specific tracing source, collected under the <c>LupiraCareerApi.*</c> source wildcard.</summary>
public static class Telemetry
{
    public const string ActivitySourceName = "LupiraCareerApi.Career";
    public static readonly ActivitySource Source = new(ActivitySourceName);
}
