namespace LupiraCareerApi.Core.Domain.Engagements.Events;

public sealed record TitleAssumed(Guid EngagementId, Guid TitleId, string Text, DateOnly EffectiveFrom);
