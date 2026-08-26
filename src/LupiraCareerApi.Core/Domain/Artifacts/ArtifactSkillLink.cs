namespace LupiraCareerApi.Core.Domain.Artifacts;

public sealed class ArtifactSkillLink
{
    public Guid SkillId { get; set; }

    public ArtifactRole Role { get; set; }
}
