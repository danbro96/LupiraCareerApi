namespace LupiraCareerApi.Core.Domain.Goals.Events;

public sealed record GoalAbandoned(
    Guid GoalId,
    DateOnly AbandonedOn,
    string Reason,
    DateTimeOffset OccurredAt);
