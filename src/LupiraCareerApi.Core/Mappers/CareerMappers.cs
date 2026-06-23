using LupiraCareerApi.Domain;
using LupiraCareerApi.Dtos;

namespace LupiraCareerApi.Mappers;

/// <summary>Maps event-sourced aggregates + documents to their wire DTOs. Pure, allocation-light projections —
/// the only place the read shape is assembled, so REST and MCP return identical structures.</summary>
public static class CareerMappers
{
    public static EngagementDto ToDto(this Engagement e, string? organizationName = null) => new()
    {
        Id = e.Id,
        Kind = e.Kind,
        OrganizationId = e.OrganizationId,
        OrganizationName = organizationName,
        Start = e.Start,
        End = e.End,
        Location = e.Location,
        Summary = e.Summary,
        CurrentTitle = e.CurrentTitle,
        Titles = [.. e.Titles.Select(t => new TitleEpochDto { TitleId = t.TitleId, Text = t.Text, From = t.From, To = t.To })],
        SkillIds = [.. e.SkillIds],
    };

    public static ProjectDto ToDto(this Project p) => new()
    {
        Id = p.Id,
        Kind = p.Kind,
        Title = p.Title,
        Description = p.Description,
        Url = p.Url,
        EngagementId = p.EngagementId,
        Start = p.Start,
        End = p.End,
        Outcome = p.Outcome,
        Status = p.Status,
        SkillIds = [.. p.SkillIds],
    };

    public static SkillDto ToDto(this Skill s) => new()
    {
        Id = s.Id,
        Name = s.Name,
        Category = s.Category,
        Aliases = [.. s.Aliases],
        ParentSkillId = s.ParentSkillId,
        Retired = s.Retired,
        FirstLearnedOn = s.FirstLearnedOn,
        CurrentMaturity = s.CurrentMaturity,
    };

    public static GoalDto ToDto(this Goal g) => new()
    {
        Id = g.Id,
        SkillId = g.SkillId,
        TargetMaturity = g.TargetMaturity,
        Deadline = g.Deadline,
        Motivation = g.Motivation,
        Status = g.Status,
        ResolvedAt = g.ResolvedAt,
        ResolutionReason = g.ResolutionReason,
        EvidenceArtifactId = g.EvidenceArtifactId,
        Progress = [.. g.Progress],
    };

    public static ArtifactDto ToDto(this Artifact a) => new()
    {
        Id = a.Id,
        Kind = a.Kind,
        Url = a.Url,
        Title = a.Title,
        Description = a.Description,
        ProducedOn = a.ProducedOn,
        Archived = a.Archived,
        LinkedProjectIds = [.. a.LinkedProjectIds],
        LinkedEngagementIds = [.. a.LinkedEngagementIds],
        LinkedSkills = [.. a.LinkedSkills],
    };

    public static MediaDto ToDto(this MediaAsset m) => new()
    {
        Id = m.Id,
        BlobRef = m.BlobRef,
        MimeType = m.MimeType,
        Width = m.Width,
        Height = m.Height,
        AltText = m.AltText,
        Caption = m.Caption,
        Archived = m.Archived,
        LinkedProjects = [.. m.LinkedProjects],
        LinkedSkillIds = [.. m.LinkedSkillIds],
    };

    public static OrganizationDto ToDto(this Organization o) => new()
    {
        Id = o.Id,
        Name = o.Name,
        Kind = o.Kind,
        Url = o.Url,
        CalContactGroupRef = o.CalContactGroupRef,
    };

    public static ProfileDto ToDto(this Profile p) => new()
    {
        OwnerPrincipalId = p.OwnerPrincipalId,
        FullName = p.FullName,
        Tagline = p.Tagline,
        Bio = p.Bio,
        Location = p.Location,
        GithubUrl = p.GithubUrl,
        LinkedInUrl = p.LinkedInUrl,
        WebsiteUrl = p.WebsiteUrl,
        PublicHandle = p.PublicHandle,
        IsPublished = p.IsPublished,
    };
}
