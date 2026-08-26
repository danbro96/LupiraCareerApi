namespace LupiraCareerApi.Core.Domain.Artifacts.Events;

public sealed record ArtifactArchived(
    Guid ArtifactId,
    string? Reason,
    DateTimeOffset OccurredAt);
