using LupiraCareerApi.Core.Domain.Shared;

namespace LupiraCareerApi.Core.Domain.Skills.Events;

public sealed record SkillApplied(
    Guid SkillId,
    DateOnly OccurredOn,
    Intensity Intensity,
    SkillEdgeContext Context,
    Evidence? Evidence,
    Location? Location);
