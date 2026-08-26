using LupiraCareerApi.Core.Domain.Artifacts;

namespace LupiraCareerApi.Core.Dtos;

public sealed class RegisterArtifactRequest
{
    public required ArtifactKind Kind { get; set; }
    public required string Url { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateOnly? ProducedOn { get; set; }
}
