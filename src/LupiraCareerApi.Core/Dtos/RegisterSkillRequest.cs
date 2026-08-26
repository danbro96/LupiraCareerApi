using LupiraCareerApi.Core.Domain.Skills;

namespace LupiraCareerApi.Core.Dtos;

public sealed class RegisterSkillRequest
{
    public required string Name { get; set; }
    public required SkillCategory Category { get; set; }
    public IReadOnlyList<string>? Aliases { get; set; }
    public Guid? ParentSkillId { get; set; }
}
