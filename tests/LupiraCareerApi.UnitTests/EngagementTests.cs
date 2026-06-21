using LupiraCareerApi.Domain;
using Xunit;

namespace LupiraCareerApi.UnitTests;

public class EngagementTests
{
    private static (Engagement e, Guid id, Guid owner, Guid org) Started()
    {
        var e = new Engagement();
        var (id, owner, org) = (Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        e.Apply(new EngagementStarted(id, owner, EngagementKind.Employment, org, new DateOnly(2020, 1, 1), null, "Backend work"));
        return (e, id, owner, org);
    }

    [Fact]
    public void Started_sets_owner_organization_and_dates()
    {
        var (e, id, owner, org) = Started();
        Assert.Equal(id, e.Id);
        Assert.Equal(owner, e.OwnerPrincipalId);
        Assert.Equal(org, e.OrganizationId);
        Assert.Equal(new DateOnly(2020, 1, 1), e.Start);
        Assert.Equal("Backend work", e.Summary);
        Assert.Null(e.End);
    }

    [Fact]
    public void AssumeTitle_closes_the_open_epoch_and_tracks_current_title()
    {
        var (e, id, _, _) = Started();
        var t1 = Guid.NewGuid();
        var t2 = Guid.NewGuid();
        e.Apply(new TitleAssumed(id, t1, "Developer", new DateOnly(2020, 1, 1)));
        e.Apply(new TitleAssumed(id, t2, "Senior Developer", new DateOnly(2022, 6, 1)));

        Assert.Equal("Senior Developer", e.CurrentTitle);
        Assert.Equal(new DateOnly(2022, 6, 1), e.Titles.Single(t => t.TitleId == t1).To);
        Assert.Null(e.Titles.Single(t => t.TitleId == t2).To);
    }

    [Fact]
    public void Ended_and_relocated_and_reclassified_apply()
    {
        var (e, id, _, _) = Started();
        e.Apply(new EngagementEnded(id, new DateOnly(2023, 1, 1), "moved on"));
        e.Apply(new EngagementRelocated(id, new Location(LocationKind.Office, "Stockholm", "SE")));
        e.Apply(new EngagementKindReclassified(id, EngagementKind.Volunteer));

        Assert.Equal(new DateOnly(2023, 1, 1), e.End);
        Assert.Equal("Stockholm", e.Location?.City);
        Assert.Equal(EngagementKind.Volunteer, e.Kind);
    }

    [Fact]
    public void Skill_attach_is_idempotent_and_detach_removes()
    {
        var (e, id, _, _) = Started();
        var skill = Guid.NewGuid();
        e.Apply(new EngagementSkillAttached(id, skill, null));
        e.Apply(new EngagementSkillAttached(id, skill, null));
        Assert.Single(e.SkillIds);
        e.Apply(new EngagementSkillDetached(id, skill));
        Assert.Empty(e.SkillIds);
    }
}
