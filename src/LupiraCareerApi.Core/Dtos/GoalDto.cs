using LupiraCareerApi.Core.Domain.Goals;
using LupiraCareerApi.Core.Domain.Skills;

namespace LupiraCareerApi.Core.Dtos;

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

    public required IReadOnlyList<GoalProgressEntryDto> Progress { get; set; }
}
