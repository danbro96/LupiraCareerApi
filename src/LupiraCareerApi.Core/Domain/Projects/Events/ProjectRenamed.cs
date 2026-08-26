namespace LupiraCareerApi.Core.Domain.Projects.Events;

public sealed record ProjectRenamed(Guid ProjectId, string NewTitle);
