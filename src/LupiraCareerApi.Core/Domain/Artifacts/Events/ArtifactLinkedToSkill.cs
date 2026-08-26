namespace LupiraCareerApi.Core.Domain.Artifacts.Events;

public sealed record ArtifactLinkedToSkill(
    Guid ArtifactId,
    Guid SkillId,
    ArtifactRole Role,
    DateTimeOffset OccurredAt);
