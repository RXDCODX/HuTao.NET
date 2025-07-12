using System.Text.Json.Serialization;

namespace HuTao.NET.Models.HoYoLab;

public class GameList
{
    [JsonPropertyName("has_role")]
    public bool HasRole { get; set; }

    [JsonPropertyName("game_id")]
    public int GameId { get; set; }

    [JsonPropertyName("game_role_id")]
    public string? GameUid { get; set; }

    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    [JsonPropertyName("region")]
    public string? Region { get; set; }

    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("background_image")]
    public string? BackgroundImage { get; set; }

    [JsonPropertyName("is_public")]
    public bool IsPublic { get; set; }

    [JsonPropertyName("data")]
    public List<GameData>? Data { get; set; }

    [JsonPropertyName("region_name")]
    public string? RegionName { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("data_switches")]
    public List<DataSwitch>? DataSwitches { get; set; }

    [JsonPropertyName("h5_data_switches")]
    public List<object>? H5DataSwitches { get; set; }

    [JsonPropertyName("background_color")]
    public string? BackgroundColor { get; set; }
}
