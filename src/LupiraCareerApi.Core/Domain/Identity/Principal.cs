namespace LupiraCareerApi.Domain.Identity;

/// <summary>
/// The identity anchor. JIT-provisioned on first login by Authentik <c>sub</c> (durable) then email (mutable),
/// identically to the calendar API so the two services converge on the same person without a shared table.
/// </summary>
public sealed class Principal
{
    public Guid Id { get; set; }
    public string AuthentikSub { get; set; } = "";
    public string Email { get; set; } = "";
    public string? DisplayName { get; set; }

    /// <summary>First provisioned. Pre-existing rows carry a reconstructed estimate.</summary>
    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset LastSeenAt { get; set; }

}
