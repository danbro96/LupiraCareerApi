namespace LupiraCareerApi.Core.Domain.Projects.Events;

public sealed record ProjectSkillDetached(Guid ProjectId, Guid SkillId);
