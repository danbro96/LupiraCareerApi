using LupiraCareerApi.Domain;

namespace LupiraCareerApi.Dtos;

public record ArtifactDto(
    Guid Id,
    ArtifactKind Kind,
    string Url,
    string Title,
    string? Description,
    DateOnly? ProducedOn,
    bool Archived,
    IReadOnlyList<Guid> LinkedProjectIds,
    IReadOnlyList<Guid> LinkedEngagementIds,
    IReadOnlyList<ArtifactSkillLink> LinkedSkills);

public record RegisterArtifactRequest(
    ArtifactKind Kind,
    string Url,
    string Title,
    string? Description,
    DateOnly? ProducedOn);

public record UpdateArtifactRequest(string? Url, string? Title, string? Description);

/// <summary>Body for linking an artifact to a skill (the skill id is in the route).</summary>
public record ArtifactSkillRoleRequest(ArtifactRole Role);
