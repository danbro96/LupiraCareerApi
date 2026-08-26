namespace LupiraCareerApi.Core.Dtos;

public sealed class TitleEpochDto
{
    public required Guid TitleId { get; set; }

    public required string Text { get; set; }

    public required DateOnly From { get; set; }

    public DateOnly? To { get; set; }
}
