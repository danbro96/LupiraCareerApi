namespace LupiraCareerApi.Core.Dtos;

public sealed class RecordProgressRequest
{
    public required string Note { get; set; }
    public Guid? LinkedEventId { get; set; }
}
