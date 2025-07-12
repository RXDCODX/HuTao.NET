using System.Text.Json.Serialization;
using HuTao.NET.GI.Models.GenshinImpact;

namespace HuTao.NET.GI.Models.ZenlessZoneZero;

public class ZenlessStats : IHoyoLab
{
    public int retcode { get; set; }
    public string? message { get; set; }
    [JsonPropertyName("data")]
    public ZenlessStatsData? Data { get; set; }
}

public class ZenlessStatsData
{
    // TODO: Add Zenless Zone Zero specific data models when the game is released
    // This will include agent details, world exploration, etc.
}
