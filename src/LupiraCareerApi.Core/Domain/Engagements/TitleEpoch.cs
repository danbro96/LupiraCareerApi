namespace LupiraCareerApi.Core.Domain.Engagements;

public class TitleEpoch
{
    public Guid TitleId { get; set; }

    public string Text { get; set; } = string.Empty;

    public DateOnly From { get; set; }

    public DateOnly? To { get; set; }
}
