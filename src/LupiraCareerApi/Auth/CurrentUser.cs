using LupiraCareerApi.Application;
using LupiraCareerApi.Domain;
using Marten;
using System.Security.Claims;

namespace LupiraCareerApi.Auth;

/// <summary>
/// The ASP.NET half of identity: reads the calling principal's claims (OIDC JWT, or the dev header) and resolves
/// them — JIT-provisioning on first login — to the local <see cref="Principal"/> via the Core
/// <see cref="PrincipalDirectory"/>. Shares the Authentik <c>sub</c> anchor with the calendar API.
/// This is the caller's own resolution site, so it is also where <see cref="EventActor"/> stamps provenance.
/// </summary>
public sealed class CurrentUser(IHttpContextAccessor http, PrincipalDirectory directory, IDocumentSession session)
{
    public async Task<Principal> GetAsync(CancellationToken ct = default)
    {
        var principal = http.HttpContext?.User
            ?? throw new InvalidOperationException("No HTTP context available.");

        var sub = principal.FindFirstValue("sub") ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = principal.FindFirstValue("email") ?? principal.FindFirstValue(ClaimTypes.Email) ?? "";
        var name = principal.FindFirstValue("name") ?? principal.Identity?.Name;
        if (sub is null && string.IsNullOrEmpty(email))
            throw new InvalidOperationException("Authenticated principal has no subject or email claim.");

        var resolved = await directory.ResolveOrProvisionAsync(sub, email, name, ct);
        // Inferring the surface from a missing sub is only valid for the caller's own identity.
        EventActor.Stamp(session, resolved, sub is null ? EventActor.SourceDav : EventActor.SourceApi);
        return resolved;
    }
}
