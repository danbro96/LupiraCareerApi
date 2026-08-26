using LupiraCareerApi.Core.Domain.Skills;

namespace LupiraCareerApi.Core.Dtos;

/// <summary>Partial update (PATCH): only non-null fields are applied, each emitting its own domain event.</summary>
public sealed class UpdateSkillRequest
{
    public string? Name { get; set; }
    public SkillCategory? Category { get; set; }
    public Guid? ParentSkillId { get; set; }
}
