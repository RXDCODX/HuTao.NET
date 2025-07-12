using System.Text.Json.Serialization;

namespace HuTao.NET.GI.Models.HoYoLab;

public class DataSwitch
{
    [JsonPropertyName("switch_id")]
    public int? SwitchId { get; set; }

    [JsonPropertyName("is_public")]
    public bool? IsPublic { get; set; }

    [JsonPropertyName("switch_name")]
    public string? SwitchName { get; set; }
}
