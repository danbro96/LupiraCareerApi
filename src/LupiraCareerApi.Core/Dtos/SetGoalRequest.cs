using LupiraCareerApi.Core.Domain.Skills;

namespace LupiraCareerApi.Core.Dtos;

public sealed class SetGoalRequest
{
    public Guid? SkillId { get; set; }

    public required Maturity TargetMaturity { get; set; }

    public DateOnly? Deadline { get; set; }

    public required string Motivation { get; set; }
}
