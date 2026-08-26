using LupiraCareerApi.Core.Dtos;
using LupiraCareerApi.Handlers;

namespace LupiraCareerApi.Endpoints;

public static class OrganizationsEndpoints
{
    public static IEndpointRouteBuilder MapOrganizations(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/organizations").RequireAuthorization("ApiPolicy").WithTags("Organizations");

        g.MapGet(string.Empty, (OrganizationsHandler h, CancellationToken ct) => h.ListAsync(ct))
            .WithName("ListOrganizations");
        g.MapPost(string.Empty, (OrganizationsHandler h, CreateOrganizationRequest body, CancellationToken ct) => h.CreateAsync(body, ct))
            .WithName("CreateOrganization");
        g.MapGet("{id:guid}", (OrganizationsHandler h, Guid id, CancellationToken ct) => h.GetAsync(id, ct))
            .WithName("GetOrganization");
        g.MapPatch("{id:guid}", (OrganizationsHandler h, Guid id, UpdateOrganizationRequest body, CancellationToken ct) => h.UpdateAsync(id, body, ct))
            .WithName("UpdateOrganization");
        g.MapDelete("{id:guid}", (OrganizationsHandler h, Guid id, CancellationToken ct) => h.DeleteAsync(id, ct))
            .WithName("DeleteOrganization");

        return app;
    }
}
