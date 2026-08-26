namespace LupiraCareerApi.Core.Domain.Skills.Events;

public sealed record SkillReparented(Guid SkillId, Guid? NewParentSkillId);
