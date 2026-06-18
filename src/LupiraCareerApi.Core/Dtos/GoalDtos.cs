using LupiraCareerApi.Domain;

namespace LupiraCareerApi.Dtos;

public record GoalDto(
    Guid Id,
    Guid? SkillId,
    Maturity TargetMaturity,
    DateOnly? Deadline,
    string Motivation,
    GoalStatus Status,
    DateTimeOffset? ResolvedAt,
    string? ResolutionReason,
    Guid? EvidenceArtifactId,
    IReadOnlyList<GoalProgressEntry> Progress);

public record SetGoalRequest(Guid? SkillId, Maturity TargetMaturity, DateOnly? Deadline, string Motivation);

/// <summary>Partial update (PATCH): re-target the goal's maturity and/or deadline.</summary>
public record RescopeGoalRequest(Maturity? TargetMaturity, DateOnly? Deadline);

public record RecordProgressRequest(string Note, Guid? LinkedEventId);

public record AchieveGoalRequest(DateOnly AchievedOn, Guid? EvidenceArtifactId);

public record AbandonGoalRequest(string Reason);
