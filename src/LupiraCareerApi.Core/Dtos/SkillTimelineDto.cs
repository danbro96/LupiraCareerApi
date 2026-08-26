namespace LupiraCareerApi.Core.Dtos;

/// <summary>The chronological edge history of one skill.</summary>
public sealed class SkillTimelineDto
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required IReadOnlyList<SkillTimelineEntryDto> Entries { get; set; }
}
