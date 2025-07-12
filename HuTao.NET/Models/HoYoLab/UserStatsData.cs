using System.Text.Json.Serialization;

namespace HuTao.NET.Models.HoYoLab;

public class UserStatsData
{
    [JsonPropertyName("list")]
    public List<GameList>? GameLists { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("type")]
    public int? Type { get; set; }

    [JsonPropertyName("Vame")]
    public string? Value { get; set; }
}
