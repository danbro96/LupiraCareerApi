using LupiraCareerApi.Core.Domain.Shared;

namespace LupiraCareerApi.Core.Domain.Skills.Events;

public sealed record SkillReferenced(
    Guid SkillId,
    DateOnly OccurredOn,
    string Note,
    SkillEdgeContext Context,
    Evidence? Evidence,
    Location? Location);
