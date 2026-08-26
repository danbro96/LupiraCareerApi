using LupiraCareerApi.Core.Domain.Artifacts;

namespace LupiraCareerApi.Core.Dtos;

public sealed class ArtifactDto
{
    public required Guid Id { get; set; }
    public required ArtifactKind Kind { get; set; }
    public required string Url { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateOnly? ProducedOn { get; set; }
    public required bool Archived { get; set; }
    public required IReadOnlyList<Guid> LinkedProjectIds { get; set; }
    public required IReadOnlyList<Guid> LinkedEngagementIds { get; set; }
    public required IReadOnlyList<ArtifactSkillLinkDto> LinkedSkills { get; set; }
}
