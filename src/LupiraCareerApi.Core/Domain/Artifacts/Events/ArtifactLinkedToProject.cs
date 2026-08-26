namespace LupiraCareerApi.Core.Domain.Artifacts.Events;

public sealed record ArtifactLinkedToProject(
    Guid ArtifactId,
    Guid ProjectId,
    DateTimeOffset OccurredAt);
