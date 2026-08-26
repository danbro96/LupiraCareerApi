namespace LupiraCareerApi.Core.Domain.Artifacts.Events;

public sealed record ArtifactUnlinked(
    Guid ArtifactId,
    ArtifactTargetKind TargetKind,
    Guid TargetId,
    DateTimeOffset OccurredAt);
