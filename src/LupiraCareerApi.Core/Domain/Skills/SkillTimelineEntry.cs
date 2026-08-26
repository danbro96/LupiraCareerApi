namespace LupiraCareerApi.Core.Domain.Skills;

public sealed class SkillTimelineEntry
{
    public required string Kind { get; set; }
    public DateOnly OccurredOn { get; set; }
    public SkillContextKind? ContextKind { get; set; }
    public Guid? ContextId { get; set; }
    public string? ContextLabel { get; set; }
    public Intensity? Intensity { get; set; }
    public Maturity? Maturity { get; set; }
    public Guid? OtherSkillId { get; set; }
    public string? Note { get; set; }
}
