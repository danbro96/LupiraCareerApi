using LupiraCareerApi.Domain;

namespace LupiraCareerApi.Dtos;

public sealed class GoalDto
{
    public required Guid Id { get; set; }
    public Guid? SkillId { get; set; }
    public required Maturity TargetMaturity { get; set; }
    public DateOnly? Deadline { get; set; }
    public required string Motivation { get; set; }
    public required GoalStatus Status { get; set; }
    public DateTimeOffset? ResolvedAt { get; set; }
    public string? ResolutionReason { get; set; }
    public Guid? EvidenceArtifactId { get; set; }
    public required IReadOnlyList<GoalProgressEntry> Progress { get; set; }
}

public sealed class SetGoalRequest
{
    public Guid? SkillId { get; set; }
    public required Maturity TargetMaturity { get; set; }
    public DateOnly? Deadline { get; set; }
    public required string Motivation { get; set; }
}

/// <summary>Partial update (PATCH): only non-null fields are applied, each emitting its own domain event.</summary>
public sealed class RescopeGoalRequest
{
    public Maturity? TargetMaturity { get; set; }
    public DateOnly? Deadline { get; set; }
}

public sealed class RecordProgressRequest
{
    public required string Note { get; set; }
    public Guid? LinkedEventId { get; set; }
}

public sealed class AchieveGoalRequest
{
    public required DateOnly AchievedOn { get; set; }
    public Guid? EvidenceArtifactId { get; set; }
}

public sealed class AbandonGoalRequest
{
    public required string Reason { get; set; }
}
