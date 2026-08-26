using LupiraCareerApi.Core.Domain.Organizations;

namespace LupiraCareerApi.Core.Dtos;

public sealed class CreateOrganizationRequest
{
    public required string Name { get; set; }

    public required OrganizationKind Kind { get; set; }

    public string? Url { get; set; }

    public Guid? CalContactGroupRef { get; set; }
}
