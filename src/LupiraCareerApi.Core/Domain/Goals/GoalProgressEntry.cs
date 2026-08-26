namespace LupiraCareerApi.Core.Domain.Goals;

public sealed class GoalProgressEntry
{
    public DateTimeOffset RecordedAt { get; set; }
    public string Note { get; set; } = "";
    public Guid? LinkedEventId { get; set; }
}
