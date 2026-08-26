namespace LupiraCareerApi.Core.Domain.Artifacts;

public sealed record ArtifactRegistered(
    Guid ArtifactId,
    Guid OwnerPrincipalId,
    ArtifactKind Kind,
    string Url,
    string Title,
    string? Description,
    DateOnly? ProducedOn,
    DateTimeOffset OccurredAt);

public sealed record ArtifactUpdated(
    Guid ArtifactId,
    string? NewUrl,
    string? NewTitle,
    string? NewDescription,
    DateTimeOffset OccurredAt);

public sealed record ArtifactLinkedToProject(
    Guid ArtifactId,
    Guid ProjectId,
    DateTimeOffset OccurredAt);

public sealed record ArtifactLinkedToSkill(
    Guid ArtifactId,
    Guid SkillId,
    ArtifactRole Role,
    DateTimeOffset OccurredAt);

public sealed record ArtifactLinkedToEngagement(
    Guid ArtifactId,
    Guid EngagementId,
    DateTimeOffset OccurredAt);

public sealed record ArtifactUnlinked(
    Guid ArtifactId,
    ArtifactTargetKind TargetKind,
    Guid TargetId,
    DateTimeOffset OccurredAt);

public sealed record ArtifactArchived(
    Guid ArtifactId,
    string? Reason,
    DateTimeOffset OccurredAt);
