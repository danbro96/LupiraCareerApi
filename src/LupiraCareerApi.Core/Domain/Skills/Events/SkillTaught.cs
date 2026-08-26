using LupiraCareerApi.Core.Domain.Shared;

namespace LupiraCareerApi.Core.Domain.Skills.Events;

public sealed record SkillTaught(
    Guid SkillId,
    DateOnly OccurredOn,
    string Audience,
    SkillEdgeContext Context,
    Evidence? Evidence,
    Location? Location);
