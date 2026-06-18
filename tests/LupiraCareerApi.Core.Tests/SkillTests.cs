using LupiraCareerApi.Domain;
using Xunit;

namespace LupiraCareerApi.Core.Tests;

public class SkillTests
{
    private static (Skill s, Guid id) Registered()
    {
        var s = new Skill();
        var id = Guid.NewGuid();
        s.Apply(new SkillRegistered(id, Guid.NewGuid(), "C#", SkillCategory.Language, ["csharp", "dotnet"], null));
        return (s, id);
    }

    [Fact]
    public void Registered_sets_name_category_and_aliases()
    {
        var (s, id) = Registered();
        Assert.Equal(id, s.Id);
        Assert.Equal("C#", s.Name);
        Assert.Equal(SkillCategory.Language, s.Category);
        Assert.Equal(2, s.Aliases.Count);
        Assert.Equal(Maturity.Aware, s.CurrentMaturity);
    }

    [Fact]
    public void Learned_sets_first_learned_on_and_initial_maturity()
    {
        var (s, id) = Registered();
        var ctx = SkillEdgeContext.External("self-study");
        s.Apply(new SkillLearned(id, new DateOnly(2018, 1, 1), Maturity.Working, ctx, null, null));
        Assert.Equal(new DateOnly(2018, 1, 1), s.FirstLearnedOn);
        Assert.Equal(Maturity.Working, s.CurrentMaturity);

        // A later learning does not move FirstLearnedOn back/forward.
        s.Apply(new SkillLearned(id, new DateOnly(2019, 1, 1), Maturity.Fluent, ctx, null, null));
        Assert.Equal(new DateOnly(2018, 1, 1), s.FirstLearnedOn);
    }

    [Fact]
    public void Deepened_advances_current_maturity()
    {
        var (s, id) = Registered();
        var ctx = SkillEdgeContext.External("work");
        s.Apply(new SkillLearned(id, new DateOnly(2018, 1, 1), Maturity.Working, ctx, null, null));
        s.Apply(new SkillDeepened(id, new DateOnly(2020, 1, 1), Maturity.Working, Maturity.Expert, "led a project", ctx, null, null));
        Assert.Equal(Maturity.Expert, s.CurrentMaturity);
    }

    [Fact]
    public void Alias_add_is_deduplicated_and_retire_flags()
    {
        var (s, id) = Registered();
        s.Apply(new SkillAliasAdded(id, "csharp"));
        Assert.Equal(2, s.Aliases.Count);
        s.Apply(new SkillAliasAdded(id, "c-sharp"));
        Assert.Equal(3, s.Aliases.Count);
        s.Apply(new SkillRetired(id));
        Assert.True(s.Retired);
    }
}
