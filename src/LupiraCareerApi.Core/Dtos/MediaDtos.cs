using LupiraCareerApi.Domain;

namespace LupiraCareerApi.Dtos;

public record MediaDto(
    Guid Id,
    string BlobRef,
    string MimeType,
    int? Width,
    int? Height,
    string AltText,
    string? Caption,
    bool Archived,
    IReadOnlyList<ProjectLink> LinkedProjects,
    IReadOnlyList<Guid> LinkedSkillIds);

public record RegisterMediaRequest(
    string BlobRef,
    string MimeType,
    int? Width,
    int? Height,
    string AltText,
    string? Caption);

/// <summary>Body for linking a media asset to a project (the project id is in the route).</summary>
public record MediaProjectRoleRequest(MediaRole Role);
