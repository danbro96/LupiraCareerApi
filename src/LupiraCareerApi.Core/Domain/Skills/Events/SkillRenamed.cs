namespace LupiraCareerApi.Core.Domain.Skills.Events;

public sealed record SkillRenamed(Guid SkillId, string NewName);
