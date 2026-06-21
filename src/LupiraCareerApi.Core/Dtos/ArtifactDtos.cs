using LupiraCareerApi.Domain;

namespace LupiraCareerApi.Dtos;

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
    public required IReadOnlyList<ArtifactSkillLink> LinkedSkills { get; set; }
}

public sealed class RegisterArtifactRequest
{
    public required ArtifactKind Kind { get; set; }
    public required string Url { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateOnly? ProducedOn { get; set; }
}

public sealed class UpdateArtifactRequest
{
    public string? Url { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
}

/// <summary>Body for linking an artifact to a skill (the skill id is in the route).</summary>
public sealed class ArtifactSkillRoleRequest
{
    public required ArtifactRole Role { get; set; }
}
