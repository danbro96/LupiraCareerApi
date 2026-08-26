namespace LupiraCareerApi.Core.Dtos;

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
    public required IReadOnlyList<ProjectLinkDto> LinkedProjects { get; set; }
    public required IReadOnlyList<Guid> LinkedSkillIds { get; set; }
}
