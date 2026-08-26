using LupiraCareerApi.Core.Dtos;
using LupiraCareerApi.Handlers;
using LupiraCareerApi.Core.Domain.Profiles;

namespace LupiraCareerApi.Endpoints;

public static class ProfileEndpoints
{
    public static IEndpointRouteBuilder MapProfile(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/profile").RequireAuthorization("ApiPolicy").WithTags("Profile");

        g.MapGet("", (ProfileHandler h, CancellationToken ct) => h.GetAsync(ct))
            .WithSummary("The caller's profile (an empty shell if none exists yet).")
            .WithName("GetProfile");

        g.MapPut("", (ProfileHandler h, UpdateProfileRequest body, CancellationToken ct) => h.UpsertAsync(body, ct))
            .WithSummary("Create or replace the caller's profile.")
            .WithName("UpsertProfile");

        return app;
    }
}
