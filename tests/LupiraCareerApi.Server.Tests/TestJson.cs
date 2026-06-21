using System.Text.Json;
using System.Text.Json.Serialization;

namespace LupiraCareerApi.Server.Tests;

/// <summary>JSON options for the test client so it reads/writes enums as names, matching the API's wire contract.</summary>
internal static class TestJson
{
    public static readonly JsonSerializerOptions Options =
        new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };
}
