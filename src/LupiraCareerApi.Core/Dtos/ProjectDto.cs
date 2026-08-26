using LupiraCareerApi.Core.Domain.Projects;

namespace LupiraCareerApi.Core.Dtos;

public sealed class ProjectDto
{
    public required Guid Id { get; set; }

    public required ProjectKind Kind { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public string? Url { get; set; }

    public Guid? EngagementId { get; set; }

    public DateOnly? Start { get; set; }

    public DateOnly? End { get; set; }

    public string? Outcome { get; set; }

    public required ProjectStatus Status { get; set; }

    public required IReadOnlyList<Guid> SkillIds { get; set; }
}
