namespace LupiraCareerApi.Domain;

public enum SkillContextKind
{
    InEngagement,
    InProject,
    External,
}

public enum EvidenceKind
{
    Url,
    Commit,
    DocRef,
    Free,
}

public enum Intensity
{
    Touched,
    Regular,
    Core,
}

public enum Maturity
{
    Aware,
    Working,
    Fluent,
    Expert,
    Teaching,
}

/// <summary>Where a skill edge happened. <see cref="External"/> with <see cref="ExternalUrl"/> is also the soft,
/// one-way link to a calendar API entity (e.g. a cal item that evidences applying the skill).</summary>
public sealed record SkillEdgeContext(
    SkillContextKind Kind,
    Guid? EngagementId = null,
    Guid? ProjectId = null,
    string? ExternalLabel = null,
    string? ExternalUrl = null)
{
    public static SkillEdgeContext InEngagement(Guid engagementId) =>
        new(SkillContextKind.InEngagement, EngagementId: engagementId);

    public static SkillEdgeContext InProject(Guid projectId) =>
        new(SkillContextKind.InProject, ProjectId: projectId);

    public static SkillEdgeContext External(string label, string? url = null) =>
        new(SkillContextKind.External, ExternalLabel: label, ExternalUrl: url);
}

public sealed record Evidence(EvidenceKind Kind, string Value);
