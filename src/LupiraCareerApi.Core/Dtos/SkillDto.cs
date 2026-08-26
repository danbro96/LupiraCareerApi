using LupiraCareerApi.Core.Domain.Skills;

namespace LupiraCareerApi.Core.Dtos;

public sealed class SkillDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required SkillCategory Category { get; set; }
    public required IReadOnlyList<string> Aliases { get; set; }
    public Guid? ParentSkillId { get; set; }
    public required bool Retired { get; set; }
    public DateOnly? FirstLearnedOn { get; set; }
    public required Maturity CurrentMaturity { get; set; }
}
