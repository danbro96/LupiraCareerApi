namespace LupiraCareerApi.Core.Domain.Projects.Events;

public sealed record ProjectShelved(Guid ProjectId, string? Reason);
