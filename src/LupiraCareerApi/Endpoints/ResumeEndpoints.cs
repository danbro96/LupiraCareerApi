using LupiraCareerApi.Handlers;

namespace LupiraCareerApi.Endpoints;

public static class ResumeEndpoints
{
    public static IEndpointRouteBuilder MapResume(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/resume", (ResumeHandler h, CancellationToken ct) => h.GetResumeAsync(ct))
            .RequireAuthorization("ApiPolicy")
            .WithTags("Resume")
            .WithSummary("The caller's full composed résumé (profile + engagements + projects + skills).");

        app.MapGet("/api/experience", (ResumeHandler h, CancellationToken ct) => h.GetExperienceAsync(ct))
            .RequireAuthorization("ApiPolicy")
            .WithTags("Resume")
            .WithSummary("The caller's unified experience timeline (engagements + projects).");

        return app;
    }
}
