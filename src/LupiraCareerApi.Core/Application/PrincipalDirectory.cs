using System.Diagnostics;
using LupiraCareerApi.Domain;
using Marten;

namespace LupiraCareerApi.Application;

/// <summary>
/// Resolves an authenticated principal (OIDC <c>sub</c> + email) to a local <see cref="Principal"/> document,
/// JIT-provisioning on first sight. Resolves by <c>sub</c> first, then email — identical to the calendar API, so
/// the two services converge on the same person.
/// </summary>
public sealed class PrincipalDirectory(IDocumentSession session)
{
    public async Task<Principal?> FindByEmailAsync(string email, CancellationToken ct = default)
    {
        email = email.Trim().ToLowerInvariant();
        if (email.Length == 0) return null;
        return await session.Query<Principal>().FirstOrDefaultAsync(x => x.Email == email, ct);
    }

    public async Task<Principal> ResolveOrProvisionAsync(string? sub, string email, string? name, CancellationToken ct = default)
    {
        email = email.Trim().ToLowerInvariant();

        Principal? p = null;
        if (sub is not null) p = await session.Query<Principal>().FirstOrDefaultAsync(x => x.AuthentikSub == sub, ct);
        if (p is null && email.Length > 0) p = await session.Query<Principal>().FirstOrDefaultAsync(x => x.Email == email, ct);

        if (p is null)
        {
            p = new Principal { Id = Guid.NewGuid(), AuthentikSub = sub ?? $"email|{email}", Email = email, DisplayName = name };
            StampSession(p, sub);
            session.Store(p);
            await session.SaveChangesAsync(ct);
            return p;
        }

        StampSession(p, sub);
        var changed = false;
        if (sub is not null && p.AuthentikSub != sub && p.AuthentikSub.StartsWith("email|", StringComparison.Ordinal)) { p.AuthentikSub = sub; changed = true; }
        if (email.Length > 0 && p.Email != email) { p.Email = email; changed = true; }
        if (name is not null && p.DisplayName != name) { p.DisplayName = name; changed = true; }
        if (changed) { session.Store(p); await session.SaveChangesAsync(ct); }
        return p;
    }

    /// <summary>Stamps the acting principal + live trace ids onto the write session so every event appended later in
    /// this request carries provenance (actor / correlation / causation / source). Runs before any append — this is
    /// the one point every surface funnels through — because provenance is unbackfillable.</summary>
    private void StampSession(Principal principal, string? sub)
    {
        session.LastModifiedBy = principal.Id.ToString();
        if (Activity.Current is { } a)
        {
            session.CorrelationId = a.TraceId.ToString();
            session.CausationId = a.SpanId.ToString();
        }
        session.SetHeader("actor.email", principal.Email);
        session.SetHeader("source", sub is null ? "dav" : "api");   // no OIDC sub ⇒ an email-only (e.g. DAV) login
    }
}
