using LupiraCareerApi.Core.Domain.Skills;

namespace LupiraCareerApi.Core.Dtos;

/// <summary>A skill's current maturity plus the trajectory that produced it.</summary>
public sealed class SkillMaturityDto
{
    public required Guid Id { get; set; }

    public required Maturity Current { get; set; }

    public required IReadOnlyList<SkillMaturityPointDto> Trajectory { get; set; }
}
