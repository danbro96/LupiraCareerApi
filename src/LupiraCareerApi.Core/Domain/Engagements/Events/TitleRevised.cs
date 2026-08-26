namespace LupiraCareerApi.Core.Domain.Engagements.Events;

public sealed record TitleRevised(Guid EngagementId, Guid TitleId, string NewText);
