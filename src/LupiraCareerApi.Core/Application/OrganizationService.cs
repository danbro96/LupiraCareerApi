using LupiraCareerApi.Domain;
using LupiraCareerApi.Dtos;
using LupiraCareerApi.Mappers;
using Marten;

namespace LupiraCareerApi.Application;

/// <summary>The caller's employers / institutions of record. Owned per-principal; an organization can't be deleted
/// while an engagement still references it.</summary>
public sealed class OrganizationService(IDocumentSession session)
{
    public async Task<OpResult<List<OrganizationDto>>> ListAsync(Guid ownerId, CancellationToken ct = default)
    {
        var orgs = await session.Query<Organization>().Where(o => o.OwnerPrincipalId == ownerId).ToListAsync(ct);
        return OpResult<List<OrganizationDto>>.Ok([.. orgs.OrderBy(o => o.Name).Select(o => o.ToDto())]);
    }

    public async Task<OpResult<OrganizationDto>> GetAsync(Guid ownerId, Guid id, CancellationToken ct = default)
    {
        var o = await session.LoadAsync<Organization>(id, ct);
        return o is null || o.OwnerPrincipalId != ownerId ? OpResult<OrganizationDto>.NotFound() : OpResult<OrganizationDto>.Ok(o.ToDto());
    }

    public async Task<OpResult<OrganizationDto>> CreateAsync(Guid ownerId, CreateOrganizationRequest r, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(r.Name)) return OpResult<OrganizationDto>.Invalid("Name is required.");
        var o = new Organization
        {
            Id = Guid.NewGuid(),
            OwnerPrincipalId = ownerId,
            Name = r.Name.Trim(),
            Kind = r.Kind,
            Url = r.Url,
            CalContactGroupRef = r.CalContactGroupRef,
        };
        session.Store(o);
        await session.SaveChangesAsync(ct);
        return OpResult<OrganizationDto>.Ok(o.ToDto());
    }

    public async Task<OpResult<OrganizationDto>> UpdateAsync(Guid ownerId, Guid id, UpdateOrganizationRequest r, CancellationToken ct = default)
    {
        var o = await session.LoadAsync<Organization>(id, ct);
        if (o is null || o.OwnerPrincipalId != ownerId) return OpResult<OrganizationDto>.NotFound();
        if (r.Name is not null) o.Name = r.Name.Trim();
        if (r.Kind is OrganizationKind kind) o.Kind = kind;
        if (r.Url is not null) o.Url = r.Url;
        if (r.CalContactGroupRef is not null) o.CalContactGroupRef = r.CalContactGroupRef;
        session.Store(o);
        await session.SaveChangesAsync(ct);
        return OpResult<OrganizationDto>.Ok(o.ToDto());
    }

    public async Task<OpResult> DeleteAsync(Guid ownerId, Guid id, CancellationToken ct = default)
    {
        var o = await session.LoadAsync<Organization>(id, ct);
        if (o is null || o.OwnerPrincipalId != ownerId) return OpResult.NotFound();
        if (await session.Query<Engagement>().AnyAsync(e => e.OrganizationId == id, ct))
            return OpResult.Conflict("Organization is still referenced by an engagement.");
        session.Delete(o);
        await session.SaveChangesAsync(ct);
        return OpResult.Ok();
    }
}
