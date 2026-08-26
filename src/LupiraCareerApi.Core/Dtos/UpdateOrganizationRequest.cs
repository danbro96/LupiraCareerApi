using LupiraCareerApi.Core.Domain.Organizations;

namespace LupiraCareerApi.Core.Dtos;

public sealed class UpdateOrganizationRequest
{
    public string? Name { get; set; }
    public OrganizationKind? Kind { get; set; }
    public string? Url { get; set; }
    public Guid? CalContactGroupRef { get; set; }
}
