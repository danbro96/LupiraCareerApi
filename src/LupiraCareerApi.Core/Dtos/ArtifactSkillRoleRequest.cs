using LupiraCareerApi.Core.Domain.Artifacts;

namespace LupiraCareerApi.Core.Dtos;

/// <summary>Body for linking an artifact to a skill (the skill id is in the route).</summary>
public sealed class ArtifactSkillRoleRequest
{
    public required ArtifactRole Role { get; set; }
}
