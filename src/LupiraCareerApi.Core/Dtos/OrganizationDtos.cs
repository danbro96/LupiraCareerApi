using LupiraCareerApi.Domain;

namespace LupiraCareerApi.Dtos;

public record OrganizationDto(Guid Id, string Name, OrganizationKind Kind, string? Url, Guid? CalContactGroupRef);

public record CreateOrganizationRequest(string Name, OrganizationKind Kind, string? Url, Guid? CalContactGroupRef);

public record UpdateOrganizationRequest(string? Name, OrganizationKind? Kind, string? Url, Guid? CalContactGroupRef);
