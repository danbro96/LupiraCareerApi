namespace LupiraCareerApi.Core.Dtos;

/// <summary>A composed résumé: the public profile header plus the published engagements, projects, and skills.</summary>
public sealed class ResumeDto
{
    public required ProfileDto Profile { get; set; }
    public required IReadOnlyList<EngagementDto> Engagements { get; set; }
    public required IReadOnlyList<ProjectDto> Projects { get; set; }
    public required IReadOnlyList<SkillDto> Skills { get; set; }
}
