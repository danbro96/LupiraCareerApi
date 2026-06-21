using LupiraCareerApi.Domain;

namespace LupiraCareerApi.Dtos;

public sealed class OrganizationDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required OrganizationKind Kind { get; set; }
    public string? Url { get; set; }
    public Guid? CalContactGroupRef { get; set; }
}

public sealed class CreateOrganizationRequest
{
    public required string Name { get; set; }
    public required OrganizationKind Kind { get; set; }
    public string? Url { get; set; }
    public Guid? CalContactGroupRef { get; set; }
}

public sealed class UpdateOrganizationRequest
{
    public string? Name { get; set; }
    public OrganizationKind? Kind { get; set; }
    public string? Url { get; set; }
    public Guid? CalContactGroupRef { get; set; }
}
