using LupiraCareerApi.Domain;
using Xunit;

namespace LupiraCareerApi.Core.Tests;

public class ProjectTests
{
    private static (Project p, Guid id) Started()
    {
        var p = new Project();
        var id = Guid.NewGuid();
        p.Apply(new ProjectStarted(id, Guid.NewGuid(), ProjectKind.Professional, "Rebuild", "desc", null, "https://x", new DateOnly(2021, 3, 1)));
        return (p, id);
    }

    [Fact]
    public void Started_defaults_to_active()
    {
        var (p, _) = Started();
        Assert.Equal("Rebuild", p.Title);
        Assert.Equal(ProjectStatus.Active, p.Status);
        Assert.Equal(new DateOnly(2021, 3, 1), p.Start);
    }

    [Fact]
    public void Shipped_sets_status_end_and_outcome()
    {
        var (p, id) = Started();
        p.Apply(new ProjectShipped(id, new DateOnly(2021, 9, 1), "launched"));
        Assert.Equal(ProjectStatus.Shipped, p.Status);
        Assert.Equal(new DateOnly(2021, 9, 1), p.End);
        Assert.Equal("launched", p.Outcome);
    }

    [Fact]
    public void Engagement_attach_and_detach()
    {
        var (p, id) = Started();
        var eng = Guid.NewGuid();
        p.Apply(new ProjectAttachedToEngagement(id, eng));
        Assert.Equal(eng, p.EngagementId);
        p.Apply(new ProjectDetachedFromEngagement(id));
        Assert.Null(p.EngagementId);
    }
}
