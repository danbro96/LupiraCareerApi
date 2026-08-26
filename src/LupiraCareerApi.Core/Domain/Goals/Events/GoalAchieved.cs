namespace LupiraCareerApi.Core.Domain.Goals.Events;

public sealed record GoalAchieved(
    Guid GoalId,
    DateOnly AchievedOn,
    Guid? EvidenceArtifactId,
    DateTimeOffset OccurredAt);
