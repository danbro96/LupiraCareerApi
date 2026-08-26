using LupiraCareerApi.Handlers;

namespace LupiraCareerApi.Endpoints;

/// <summary>The public, handle-addressed read surface (<c>/public/{handle}</c>). Mirrors the slice of the owner
/// surface LupiraWeb consumes, but the owner is resolved from the handle and the items are filtered to the published
/// subset. Gated by <c>PublicReadPolicy</c> (a valid token, not owner-scoped) — there are still no anonymous reads.</summary>
public static class PublicEndpoints
{
    public static IEndpointRouteBuilder MapPublicPortfolio(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/public/{handle}").RequireAuthorization("PublicReadPolicy").WithTags("Public");

        g.MapGet(string.Empty, (PublicPortfolioHandler h, string handle, CancellationToken ct) => h.GetPortfolioAsync(handle, ct))
            .WithSummary("A published portfolio (profile + engagements + projects + skills + experience + media + artifacts), filtered to public items.")
            .WithName("GetPublicPortfolio");

        g.MapGet("profile", (PublicPortfolioHandler h, string handle, CancellationToken ct) => h.GetProfileAsync(handle, ct))
            .WithName("GetPublicProfile");

        g.MapGet("engagements", (PublicPortfolioHandler h, string handle, CancellationToken ct) => h.ListEngagementsAsync(handle, ct))
            .WithName("ListPublicEngagements");
        g.MapGet("engagements/{id:guid}", (PublicPortfolioHandler h, string handle, Guid id, CancellationToken ct) => h.GetEngagementAsync(handle, id, ct))
            .WithName("GetPublicEngagement");

        g.MapGet("projects", (PublicPortfolioHandler h, string handle, CancellationToken ct) => h.ListProjectsAsync(handle, ct))
            .WithName("ListPublicProjects");
        g.MapGet("projects/{id:guid}", (PublicPortfolioHandler h, string handle, Guid id, CancellationToken ct) => h.GetProjectAsync(handle, id, ct))
            .WithName("GetPublicProject");

        g.MapGet("skills", (PublicPortfolioHandler h, string handle, CancellationToken ct) => h.ListSkillsAsync(handle, ct))
            .WithName("ListPublicSkills");
        g.MapGet("skills/{id:guid}", (PublicPortfolioHandler h, string handle, Guid id, CancellationToken ct) => h.GetSkillAsync(handle, id, ct))
            .WithName("GetPublicSkill");
        g.MapGet("skills/{id:guid}/timeline", (PublicPortfolioHandler h, string handle, Guid id, CancellationToken ct) => h.SkillTimelineAsync(handle, id, ct))
            .WithName("GetPublicSkillTimeline");
        g.MapGet("skills/{id:guid}/maturity", (PublicPortfolioHandler h, string handle, Guid id, CancellationToken ct) => h.SkillMaturityAsync(handle, id, ct))
            .WithName("GetPublicSkillMaturity");

        g.MapGet("experience", (PublicPortfolioHandler h, string handle, CancellationToken ct) => h.GetExperienceAsync(handle, ct))
            .WithName("GetPublicExperience");

        g.MapGet("media", (PublicPortfolioHandler h, string handle, CancellationToken ct) => h.ListMediaAsync(handle, ct))
            .WithName("ListPublicMedia");
        g.MapGet("media/{id:guid}", (PublicPortfolioHandler h, string handle, Guid id, CancellationToken ct) => h.GetMediaAsync(handle, id, ct))
            .WithName("GetPublicMedia");

        g.MapGet("artifacts", (PublicPortfolioHandler h, string handle, CancellationToken ct) => h.ListArtifactsAsync(handle, ct))
            .WithName("ListPublicArtifacts");
        g.MapGet("artifacts/{id:guid}", (PublicPortfolioHandler h, string handle, Guid id, CancellationToken ct) => h.GetArtifactAsync(handle, id, ct))
            .WithName("GetPublicArtifact");

        return app;
    }
}
