namespace LupiraCareerApi.Domain;

public enum LocationKind
{
    Office,
    Home,
    Client,
    Event,
}

/// <summary>A lightweight, denormalized location value (city/country) attached to engagements and skill edges.
/// Distinct from the calendar API's hierarchical Place catalog — the career API only records where work happened,
/// not logistics.</summary>
public record Location(LocationKind Kind, string? City, string? Country);
