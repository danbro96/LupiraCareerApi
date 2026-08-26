namespace LupiraCareerApi.Core.Domain.Skills.Events;

public sealed record SkillRegistered(
    Guid SkillId,
    Guid OwnerPrincipalId,
    string Name,
    SkillCategory Category,
    IReadOnlyList<string>? Aliases,
    Guid? ParentSkillId);
