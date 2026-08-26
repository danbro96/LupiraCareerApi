namespace LupiraCareerApi.Core.Domain.Skills.Events;

public sealed record SkillCategoryChanged(Guid SkillId, SkillCategory NewCategory);
