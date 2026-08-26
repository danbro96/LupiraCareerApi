namespace LupiraCareerApi.Core.Dtos;

public sealed class RegisterMediaRequest
{
    public required string BlobRef { get; set; }

    public required string MimeType { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public required string AltText { get; set; }

    public string? Caption { get; set; }
}
