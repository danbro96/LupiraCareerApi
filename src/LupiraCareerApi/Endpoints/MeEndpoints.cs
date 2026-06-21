using LupiraCareerApi.Handlers;

namespace LupiraCareerApi.Endpoints;

public static class MeEndpoints
{
    public static IEndpointRouteBuilder MapMe(this IEndpointRouteBuilder app)
    {
        app.MapGet("/me", (MeHandler h, CancellationToken ct) => h.GetAsync(ct))
            .RequireAuthorization("ApiPolicy")
            .WithTags("Me")
            .WithSummary("The caller's resolved local identity (JIT-provisioned on first login).");
        return app;
    }
}
