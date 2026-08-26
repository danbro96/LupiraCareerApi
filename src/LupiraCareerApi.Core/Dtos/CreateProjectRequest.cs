using LupiraCareerApi.Core.Domain.Projects;

namespace LupiraCareerApi.Core.Dtos;

public sealed class CreateProjectRequest
{
    public required ProjectKind Kind { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public Guid? EngagementId { get; set; }

    public string? Url { get; set; }

    public DateOnly? Start { get; set; }
}
