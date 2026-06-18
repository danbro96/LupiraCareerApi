using LupiraCareerApi.Domain;

namespace LupiraCareerApi.Dtos;

public record TitleEpochDto(Guid TitleId, string Text, DateOnly From, DateOnly? To);

public record EngagementDto(
    Guid Id,
    EngagementKind Kind,
    Guid OrganizationId,
    string? OrganizationName,
    DateOnly Start,
    DateOnly? End,
    Location? Location,
    string? Summary,
    string? CurrentTitle,
    IReadOnlyList<TitleEpochDto> Titles,
    IReadOnlyList<Guid> SkillIds);

public record CreateEngagementRequest(
    EngagementKind Kind,
    Guid OrganizationId,
    DateOnly Start,
    Location? Location,
    string? Summary);

/// <summary>Partial update (PATCH): only non-null fields are applied, each emitting its own domain event.
/// Supplying <see cref="End"/> ends the engagement.</summary>
public record UpdateEngagementRequest(
    string? Summary,
    EngagementKind? Kind,
    Location? Location,
    DateOnly? End,
    string? EndReason);

public record AssumeTitleRequest(string Text, DateOnly EffectiveFrom);

/// <summary>Revise the title's text and/or set the date it was retired.</summary>
public record UpdateTitleRequest(string? Text, DateOnly? RetiredOn);
