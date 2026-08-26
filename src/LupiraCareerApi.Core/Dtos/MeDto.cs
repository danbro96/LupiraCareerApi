namespace LupiraCareerApi.Core.Dtos;

public sealed class MeDto
{
    public required Guid Id { get; set; }
    public required string Email { get; set; }
    public string? DisplayName { get; set; }
}
