namespace LupiraCareerApi.Core.Domain.Engagements.Events;

public sealed record EngagementSkillAttached(Guid EngagementId, Guid SkillId, DateOnly? AttachedOn);
