namespace LupiraCareerApi.Domain;

public sealed record GoalSet(
    Guid GoalId,
    Guid OwnerPrincipalId,
    Guid? SkillId,
    Maturity TargetMaturity,
    DateOnly? Deadline,
    string Motivation,
    DateTimeOffset OccurredAt);

public sealed record GoalRescoped(
    Guid GoalId,
    Maturity? NewTargetMaturity,
    DateOnly? NewDeadline,
    DateTimeOffset OccurredAt);

public sealed record GoalProgressRecorded(
    Guid GoalId,
    string Note,
    Guid? LinkedEventId,
    DateTimeOffset OccurredAt);

public sealed record GoalAchieved(
    Guid GoalId,
    DateOnly AchievedOn,
    Guid? EvidenceArtifactId,
    DateTimeOffset OccurredAt);

public sealed record GoalAbandoned(
    Guid GoalId,
    DateOnly AbandonedOn,
    string Reason,
    DateTimeOffset OccurredAt);
