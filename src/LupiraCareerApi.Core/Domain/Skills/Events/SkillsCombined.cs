namespace LupiraCareerApi.Core.Domain.Skills.Events;

public sealed record SkillsCombined(
    Guid SkillId,
    Guid OtherSkillId,
    DateOnly OccurredOn,
    bool IsPrimary,
    SkillEdgeContext Context,
    Evidence? Evidence);
