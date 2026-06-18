namespace LupiraCareerApi.Domain;

public enum OrganizationKind
{
    Company,
    School,
    Nonprofit,
    Other,
}

/// <summary>
/// An employer / institution of record — the career API owns this concept (the calendar API's
/// <c>ContactGroup{Organization}</c> is a different thing: people you know there). Dedups employers across
/// engagements. A plain document, owned per-principal. <see cref="CalContactGroupRef"/> optionally cross-links
/// to the matching calendar contact-org via the cal API's <c>Relation(toKind="organization")</c>.
/// </summary>
public sealed class Organization
{
    public Guid Id { get; set; }
    public Guid OwnerPrincipalId { get; set; }

    public string Name { get; set; } = "";
    public OrganizationKind Kind { get; set; } = OrganizationKind.Company;
    public string? Url { get; set; }
    public Guid? CalContactGroupRef { get; set; }
}
