namespace LupiraCareerApi.Core.Domain.Goals.Events;

public sealed record GoalProgressRecorded(
    Guid GoalId,
    string Note,
    Guid? LinkedEventId,
    DateTimeOffset OccurredAt);
