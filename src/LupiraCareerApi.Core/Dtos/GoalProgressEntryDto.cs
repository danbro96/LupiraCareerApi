namespace LupiraCareerApi.Core.Dtos;

public sealed class GoalProgressEntryDto
{
    public required DateTimeOffset RecordedAt { get; set; }
    public required string Note { get; set; }
    public required Guid? LinkedEventId { get; set; }
}
