using LupiraCareerApi.Core.Domain.Skills;

namespace LupiraCareerApi.Core.Domain.Goals.Events;

public sealed record GoalRescoped(
    Guid GoalId,
    Maturity? NewTargetMaturity,
    DateOnly? NewDeadline,
    DateTimeOffset OccurredAt);
