using LupiraCareerApi.Domain;

namespace LupiraCareerApi.Dtos;

/// <summary>One row of the unified experience timeline (engagements + projects).</summary>
public sealed class ExperienceItemDto
{
    public required ExperienceKind Kind { get; set; }
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required DateOnly OccurredOn { get; set; }
    public DateOnly? EndDate { get; set; }
    public Guid? OrganizationId { get; set; }
    public string? OrganizationName { get; set; }
    public Location? Location { get; set; }
    public required IReadOnlyList<Guid> SkillIds { get; set; }
}

/// <summary>A composed résumé: the public profile header plus the published engagements, projects, and skills.</summary>
public sealed class ResumeDto
{
    public required ProfileDto Profile { get; set; }
    public required IReadOnlyList<EngagementDto> Engagements { get; set; }
    public required IReadOnlyList<ProjectDto> Projects { get; set; }
    public required IReadOnlyList<SkillDto> Skills { get; set; }
}
