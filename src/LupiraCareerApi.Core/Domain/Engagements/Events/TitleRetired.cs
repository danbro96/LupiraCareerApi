namespace LupiraCareerApi.Core.Domain.Engagements.Events;

public sealed record TitleRetired(Guid EngagementId, Guid TitleId, DateOnly EffectiveTo);
