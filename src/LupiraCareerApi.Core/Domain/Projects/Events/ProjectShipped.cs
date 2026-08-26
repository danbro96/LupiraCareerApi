namespace LupiraCareerApi.Core.Domain.Projects.Events;

public sealed record ProjectShipped(Guid ProjectId, DateOnly ShippedOn, string? Outcome);
