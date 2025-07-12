using System.Text.Json.Serialization;
using HuTao.NET.GI.Models.GenshinImpact;

namespace HuTao.NET.GI.Models.HoYoLab;

public class UserStats : IHoyoLab
{
    public int retcode { get; set; }
    public string? message { get; set; }
    [JsonPropertyName("data")]
    public UserStatsData? Data { get; set; }
}
