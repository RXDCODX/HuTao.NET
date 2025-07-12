using System.Text.Json.Serialization;
using HuTao.NET.Models.GenshinImpact;

namespace HuTao.NET.Models.ZenlessZoneZero;

public class ZenlessStats : IHoyoLab
{
    public int Retcode { get; set; }
    public string? Message { get; set; }

    [JsonPropertyName("data")]
    public ZenlessStatsData? Data { get; set; }
}

public class ZenlessStatsData
{
    // TODO: Add Zenless Zone Zero specific data models when the game is released
    // This will include agent details, world exploration, etc.
}
