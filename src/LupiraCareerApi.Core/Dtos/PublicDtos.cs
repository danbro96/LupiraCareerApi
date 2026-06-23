namespace LupiraCareerApi.Dtos;

/// <summary>The composed public portfolio for one published handle: the profile header plus the published subset of
/// each collection (archived projects/media/artifacts and retired skills filtered out; goals never included). Built
/// from the same per-resource DTOs the owner surface returns, so the wire shapes stay identical.</summary>
public sealed class PublicPortfolioDto
{
    public required ProfileDto Profile { get; set; }
    public required IReadOnlyList<EngagementDto> Engagements { get; set; }
    public required IReadOnlyList<ProjectDto> Projects { get; set; }
    public required IReadOnlyList<SkillDto> Skills { get; set; }
    public required IReadOnlyList<ExperienceItemDto> Experience { get; set; }
    public required IReadOnlyList<MediaDto> Media { get; set; }
    public required IReadOnlyList<ArtifactDto> Artifacts { get; set; }
}
