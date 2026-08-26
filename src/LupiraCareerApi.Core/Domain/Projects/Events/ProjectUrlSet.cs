namespace LupiraCareerApi.Core.Domain.Projects.Events;

public sealed record ProjectUrlSet(Guid ProjectId, string? Url);
