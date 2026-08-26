using LupiraCareerApi.Core.Domain.Artifacts;

namespace LupiraCareerApi.Core.Dtos;

public sealed class ArtifactSkillLinkDto
{
    public required Guid SkillId { get; set; }

    public required ArtifactRole Role { get; set; }
}
