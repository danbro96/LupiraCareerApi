namespace LupiraCareerApi.Core.Domain.Projects.Events;

public sealed record ProjectAttachedToEngagement(Guid ProjectId, Guid EngagementId);
