namespace LupiraCareerApi.Core.Domain.Projects.Events;

public sealed record ProjectSkillAttached(Guid ProjectId, Guid SkillId, DateOnly? AttachedOn);
