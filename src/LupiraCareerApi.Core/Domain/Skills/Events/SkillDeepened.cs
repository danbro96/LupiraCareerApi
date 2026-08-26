using LupiraCareerApi.Core.Domain.Shared;

namespace LupiraCareerApi.Core.Domain.Skills.Events;

public sealed record SkillDeepened(
    Guid SkillId,
    DateOnly OccurredOn,
    Maturity FromMaturity,
    Maturity ToMaturity,
    string? Note,
    SkillEdgeContext Context,
    Evidence? Evidence,
    Location? Location);
