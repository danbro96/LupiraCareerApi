using LupiraCareerApi.Core.Domain.Skills;

namespace LupiraCareerApi.Core.Dtos;

public sealed class SkillTimelineEntryDto
{
    public required string Kind { get; set; }

    public required DateOnly OccurredOn { get; set; }

    public required SkillContextKind? ContextKind { get; set; }

    public required Guid? ContextId { get; set; }

    public required string? ContextLabel { get; set; }

    public required Intensity? Intensity { get; set; }

    public required Maturity? Maturity { get; set; }

    public required Guid? OtherSkillId { get; set; }

    public required string? Note { get; set; }
}
