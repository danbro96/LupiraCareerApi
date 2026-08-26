namespace LupiraCareerApi.Core.Domain.Artifacts.Events;

public sealed record ArtifactLinkedToEngagement(
    Guid ArtifactId,
    Guid EngagementId,
    DateTimeOffset OccurredAt);
