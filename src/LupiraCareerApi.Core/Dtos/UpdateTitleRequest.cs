namespace LupiraCareerApi.Core.Dtos;

public sealed class UpdateTitleRequest
{
    public string? Text { get; set; }

    public DateOnly? RetiredOn { get; set; }
}
