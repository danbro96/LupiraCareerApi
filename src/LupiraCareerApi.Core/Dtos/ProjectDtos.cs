using LupiraCareerApi.Domain;

namespace LupiraCareerApi.Dtos;

public record ProjectDto(
    Guid Id,
    ProjectKind Kind,
    string Title,
    string? Description,
    string? Url,
    Guid? EngagementId,
    DateOnly? Start,
    DateOnly? End,
    string? Outcome,
    ProjectStatus Status,
    IReadOnlyList<Guid> SkillIds);

public record CreateProjectRequest(
    ProjectKind Kind,
    string Title,
    string? Description,
    Guid? EngagementId,
    string? Url,
    DateOnly? Start);

public record UpdateProjectRequest(string? Title, string? Description, string? Url);

public record ShipProjectRequest(DateOnly ShippedOn, string? Outcome);

public record AttachEngagementRequest(Guid EngagementId);
