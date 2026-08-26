namespace LupiraCareerApi.Core.Domain.Skills;

public sealed class SkillMaturityPoint
{
    public DateOnly OccurredOn { get; set; }

    public Maturity Maturity { get; set; }

    public string? Reason { get; set; }
}
