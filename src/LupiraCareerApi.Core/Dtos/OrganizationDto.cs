using LupiraCareerApi.Core.Domain.Organizations;

namespace LupiraCareerApi.Core.Dtos;

public sealed class OrganizationDto
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required OrganizationKind Kind { get; set; }

    public string? Url { get; set; }

    public Guid? CalContactGroupRef { get; set; }
}
