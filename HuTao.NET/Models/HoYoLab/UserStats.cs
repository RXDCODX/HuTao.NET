using System.Text.Json.Serialization;
using HuTao.NET.Models.GenshinImpact;

namespace HuTao.NET.Models.HoYoLab;

public class UserStats : IHoyoLab
{
    public int Retcode { get; set; }
    public string? Message { get; set; }

    [JsonPropertyName("data")]
    public UserStatsData? Data { get; set; }
}
