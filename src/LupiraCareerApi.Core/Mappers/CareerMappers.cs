using LupiraCareerApi.Domain;
using LupiraCareerApi.Dtos;

namespace LupiraCareerApi.Mappers;

/// <summary>Maps event-sourced aggregates + documents to their wire DTOs. Pure, allocation-light projections —
/// the only place the read shape is assembled, so REST and MCP return identical structures.</summary>
public static class CareerMappers
{
    public static EngagementDto ToDto(this Engagement e, string? organizationName = null) => new(
        e.Id, e.Kind, e.OrganizationId, organizationName, e.Start, e.End, e.Location, e.Summary, e.CurrentTitle,
        [.. e.Titles.Select(t => new TitleEpochDto(t.TitleId, t.Text, t.From, t.To))],
        [.. e.SkillIds]);

    public static ProjectDto ToDto(this Project p) => new(
        p.Id, p.Kind, p.Title, p.Description, p.Url, p.EngagementId, p.Start, p.End, p.Outcome, p.Status, [.. p.SkillIds]);

    public static SkillDto ToDto(this Skill s) => new(
        s.Id, s.Name, s.Category, [.. s.Aliases], s.ParentSkillId, s.Retired, s.FirstLearnedOn, s.CurrentMaturity);

    public static GoalDto ToDto(this Goal g) => new(
        g.Id, g.SkillId, g.TargetMaturity, g.Deadline, g.Motivation, g.Status, g.ResolvedAt, g.ResolutionReason,
        g.EvidenceArtifactId, [.. g.Progress]);

    public static ArtifactDto ToDto(this Artifact a) => new(
        a.Id, a.Kind, a.Url, a.Title, a.Description, a.ProducedOn, a.Archived,
        [.. a.LinkedProjectIds], [.. a.LinkedEngagementIds], [.. a.LinkedSkills]);

    public static MediaDto ToDto(this MediaAsset m) => new(
        m.Id, m.BlobRef, m.MimeType, m.Width, m.Height, m.AltText, m.Caption, m.Archived,
        [.. m.LinkedProjects], [.. m.LinkedSkillIds]);

    public static OrganizationDto ToDto(this Organization o) => new(o.Id, o.Name, o.Kind, o.Url, o.CalContactGroupRef);

    public static ProfileDto ToDto(this Profile p) => new(
        p.OwnerPrincipalId, p.FullName, p.Tagline, p.Bio, p.Location, p.GithubUrl, p.LinkedInUrl, p.WebsiteUrl);
}
