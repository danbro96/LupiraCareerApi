namespace LupiraCareerApi.Core.Dtos;

public sealed class UpdateProjectRequest
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Url { get; set; }
}
