using LupiraCareerApi.Core.Domain.Skills.Events;

namespace LupiraCareerApi.Core.Domain.Skills;

/// <summary>Inline read model: the chronological edge history of one skill. Single-stream, so it is naturally
/// owner-scoped — <see cref="OwnerPrincipalId"/> is stamped from <see cref="SkillRegistered"/>.</summary>
public sealed class SkillTimeline
{
    public Guid Id { get; set; }
    public Guid OwnerPrincipalId { get; set; }
    public string Name { get; set; } = "";
    public List<SkillTimelineEntry> Entries { get; set; } = new();
}
