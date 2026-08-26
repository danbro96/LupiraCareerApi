namespace LupiraCareerApi.Core.Domain.Projects.Events;

public sealed record ProjectDescribed(Guid ProjectId, string? Description);
