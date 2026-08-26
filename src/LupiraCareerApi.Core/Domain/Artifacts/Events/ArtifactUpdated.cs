namespace LupiraCareerApi.Core.Domain.Artifacts.Events;

public sealed record ArtifactUpdated(
    Guid ArtifactId,
    string? NewUrl,
    string? NewTitle,
    string? NewDescription,
    DateTimeOffset OccurredAt);
