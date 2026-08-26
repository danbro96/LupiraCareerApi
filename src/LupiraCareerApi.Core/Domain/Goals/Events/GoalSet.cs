using LupiraCareerApi.Core.Domain.Skills;

namespace LupiraCareerApi.Core.Domain.Goals.Events;

public sealed record GoalSet(
    Guid GoalId,
    Guid OwnerPrincipalId,
    Guid? SkillId,
    Maturity TargetMaturity,
    DateOnly? Deadline,
    string Motivation,
    DateTimeOffset OccurredAt);
