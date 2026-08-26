namespace LupiraCareerApi.Core.Dtos;

public sealed class AssumeTitleRequest
{
    public required string Text { get; set; }

    public required DateOnly EffectiveFrom { get; set; }
}
