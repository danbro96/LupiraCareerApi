using LupiraCareerApi.Core.Domain.Skills;

namespace LupiraCareerApi.Core.Dtos;

public sealed class SkillMaturityPointDto
{
    public required DateOnly OccurredOn { get; set; }
    public required Maturity Maturity { get; set; }
    public required string? Reason { get; set; }
}
