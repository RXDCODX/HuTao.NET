using System.Text.Json.Serialization;
using HuTao.NET.GI.Models.GenshinImpact;

namespace HuTao.NET.GI.Models.HonkaiStarRail;

public class StarRailRewardData : IHoyoLab
{
    public int retcode { get; set; }
    public string? message { get; set; }
    [JsonPropertyName("data")]
    public StarRailRewardDataInfo? Data { get; set; }
}

public class StarRailRewardDataInfo
{
    [JsonPropertyName("month")]
    public int Month { get; set; }

    [JsonPropertyName("awards")]
    public StarRailAward[]? Awards { get; set; }

    [JsonPropertyName("biz")]
    public string Biz { get; set; } = string.Empty;

    [JsonPropertyName("resign")]
    public bool Resign { get; set; }

    [JsonPropertyName("short_extra_award")]
    public ShortExtraAward? ShortExtraAward { get; set; }
}

public class StarRailAward
{
    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("cnt")]
    public int Count { get; set; }
}

public class ShortExtraAward
{
    [JsonPropertyName("has_extra_award")]
    public bool HasExtraAward { get; set; }

    [JsonPropertyName("start_time")]
    public string StartTime { get; set; } = string.Empty;

    [JsonPropertyName("end_time")]
    public string EndTime { get; set; } = string.Empty;

    [JsonPropertyName("list")]
    public object[]? List { get; set; }

    [JsonPropertyName("start_timestamp")]
    public string StartTimestamp { get; set; } = string.Empty;

    [JsonPropertyName("end_timestamp")]
    public string EndTimestamp { get; set; } = string.Empty;
} 