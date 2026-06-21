using LupiraCareerApi.Domain;

namespace LupiraCareerApi.Dtos;

public sealed class MediaDto
{
    public required Guid Id { get; set; }
    public required string BlobRef { get; set; }
    public required string MimeType { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public required string AltText { get; set; }
    public string? Caption { get; set; }
    public required bool Archived { get; set; }
    public required IReadOnlyList<ProjectLink> LinkedProjects { get; set; }
    public required IReadOnlyList<Guid> LinkedSkillIds { get; set; }
}

public sealed class RegisterMediaRequest
{
    public required string BlobRef { get; set; }
    public required string MimeType { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public required string AltText { get; set; }
    public string? Caption { get; set; }
}

/// <summary>Body for linking a media asset to a project (the project id is in the route).</summary>
public sealed class MediaProjectRoleRequest
{
    public required MediaRole Role { get; set; }
}
