namespace LupiraCareerApi.Core.Dtos;

public sealed class ShipProjectRequest
{
    public required DateOnly ShippedOn { get; set; }
    public string? Outcome { get; set; }
}
