namespace LupiraCareerApi.Domain;

public enum EngagementKind
{
    Employment,
    Study,
    Hobby,
    Volunteer,
    OpenSource,
}

public class TitleEpoch
{
    public Guid TitleId { get; set; }
    public string Text { get; set; } = "";
    public DateOnly From { get; set; }
    public DateOnly? To { get; set; }
}

/// <summary>An employment / study / volunteer engagement: a time-boxed relationship with an
/// <see cref="Organization"/>, carrying a history of job titles. Event-sourced; one stream per engagement,
/// owned by a single <see cref="OwnerPrincipalId"/>.</summary>
public class Engagement
{
    public Guid Id { get; set; }
    public int Version { get; set; }

    public Guid OwnerPrincipalId { get; set; }

    public EngagementKind Kind { get; set; }
    public Guid OrganizationId { get; set; }
    public DateOnly Start { get; set; }
    public DateOnly? End { get; set; }
    public Location? Location { get; set; }
    public string? Summary { get; set; }
    public List<TitleEpoch> Titles { get; set; } = new();
    public List<Guid> SkillIds { get; set; } = new();

    public string? CurrentTitle =>
        Titles.LastOrDefault(t => t.To is null)?.Text
        ?? Titles.LastOrDefault()?.Text;

    public void Apply(EngagementStarted e)
    {
        Id = e.EngagementId;
        OwnerPrincipalId = e.OwnerPrincipalId;
        Kind = e.Kind;
        OrganizationId = e.OrganizationId;
        Start = e.StartDate;
        Location = e.Location;
        Summary = e.Summary;
    }

    public void Apply(EngagementEnded e) => End = e.EndDate;
    public void Apply(EngagementSummaryRevised e) => Summary = e.Summary;
    public void Apply(EngagementRelocated e) => Location = e.NewLocation;
    public void Apply(EngagementKindReclassified e) => Kind = e.NewKind;

    public void Apply(TitleAssumed e)
    {
        var openEpoch = Titles.FirstOrDefault(t => t.To is null);
        if (openEpoch is not null)
            openEpoch.To = e.EffectiveFrom;

        Titles.Add(new TitleEpoch
        {
            TitleId = e.TitleId,
            Text = e.Text,
            From = e.EffectiveFrom,
        });
    }

    public void Apply(TitleRevised e)
    {
        var t = Titles.FirstOrDefault(x => x.TitleId == e.TitleId);
        if (t is not null) t.Text = e.NewText;
    }

    public void Apply(TitleRetired e)
    {
        var t = Titles.FirstOrDefault(x => x.TitleId == e.TitleId);
        if (t is not null) t.To = e.EffectiveTo;
    }

    public void Apply(EngagementSkillAttached e)
    {
        if (!SkillIds.Contains(e.SkillId))
            SkillIds.Add(e.SkillId);
    }

    public void Apply(EngagementSkillDetached e) => SkillIds.Remove(e.SkillId);
}
