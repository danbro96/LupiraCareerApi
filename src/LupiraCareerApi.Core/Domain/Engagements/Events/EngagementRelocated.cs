using LupiraCareerApi.Core.Domain.Shared;

namespace LupiraCareerApi.Core.Domain.Engagements.Events;

public sealed record EngagementRelocated(Guid EngagementId, Location NewLocation);
