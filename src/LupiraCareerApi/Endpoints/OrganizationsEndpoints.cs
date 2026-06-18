using LupiraCareerApi.Dtos;
using LupiraCareerApi.Handlers;

namespace LupiraCareerApi.Endpoints;

public static class OrganizationsEndpoints
{
    public static IEndpointRouteBuilder MapOrganizations(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/organizations").RequireAuthorization("ApiPolicy").WithTags("Organizations");

        g.MapGet("", (OrganizationsHandler h, CancellationToken ct) => h.ListAsync(ct));
        g.MapPost("", (OrganizationsHandler h, CreateOrganizationRequest body, CancellationToken ct) => h.CreateAsync(body, ct));
        g.MapGet("{id:guid}", (OrganizationsHandler h, Guid id, CancellationToken ct) => h.GetAsync(id, ct));
        g.MapPatch("{id:guid}", (OrganizationsHandler h, Guid id, UpdateOrganizationRequest body, CancellationToken ct) => h.UpdateAsync(id, body, ct));
        g.MapDelete("{id:guid}", (OrganizationsHandler h, Guid id, CancellationToken ct) => h.DeleteAsync(id, ct));

        return app;
    }
}
