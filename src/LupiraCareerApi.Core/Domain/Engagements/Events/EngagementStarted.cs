using LupiraCareerApi.Core.Domain.Shared;

namespace LupiraCareerApi.Core.Domain.Engagements.Events;

public sealed record EngagementStarted(
    Guid EngagementId,
    Guid OwnerPrincipalId,
    EngagementKind Kind,
    Guid OrganizationId,
    DateOnly StartDate,
    Location? Location,
    string? Summary);
