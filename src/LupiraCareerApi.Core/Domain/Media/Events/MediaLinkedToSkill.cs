namespace LupiraCareerApi.Core.Domain.Media.Events;

public sealed record MediaLinkedToSkill(
    Guid MediaId,
    Guid SkillId,
    string? Note,
    DateTimeOffset OccurredAt);
