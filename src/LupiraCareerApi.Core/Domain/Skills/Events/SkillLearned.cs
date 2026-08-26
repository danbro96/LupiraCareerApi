using LupiraCareerApi.Core.Domain.Shared;

namespace LupiraCareerApi.Core.Domain.Skills.Events;

public sealed record SkillLearned(
    Guid SkillId,
    DateOnly OccurredOn,
    Maturity InitialMaturity,
    SkillEdgeContext Context,
    Evidence? Evidence,
    Location? Location);
