using System.Text.Json.Serialization;
using HuTao.NET.GI.Models.GenshinImpact;

namespace HuTao.NET.GI.Models.HonkaiStarRail;

public class StarRailDailyNote : IHoyoLab
{
    public int retcode { get; set; }
    public string? message { get; set; }
    [JsonPropertyName("data")]
    public StarRailDailyNoteData? Data { get; set; }
}

public class StarRailDailyNoteData
{
    [JsonPropertyName("current_stamina")]
    public int CurrentStamina { get; set; }

    [JsonPropertyName("max_stamina")]
    public int MaxStamina { get; set; }

    [JsonPropertyName("stamina_recover_time")]
    public int StaminaRecoverTime { get; set; }

    [JsonPropertyName("stamina_full_ts")]
    public long StaminaFullTimestamp { get; set; }

    [JsonPropertyName("accepted_epedition_num")]
    public int AcceptedExpeditionNum { get; set; }

    [JsonPropertyName("total_expedition_num")]
    public int TotalExpeditionNum { get; set; }

    [JsonPropertyName("expeditions")]
    public StarRailExpedition[]? Expeditions { get; set; }

    [JsonPropertyName("current_train_score")]
    public int CurrentTrainScore { get; set; }

    [JsonPropertyName("max_train_score")]
    public int MaxTrainScore { get; set; }

    [JsonPropertyName("current_rogue_score")]
    public int CurrentRogueScore { get; set; }

    [JsonPropertyName("max_rogue_score")]
    public int MaxRogueScore { get; set; }

    [JsonPropertyName("weekly_cocoon_cnt")]
    public int WeeklyCocoonCount { get; set; }

    [JsonPropertyName("weekly_cocoon_limit")]
    public int WeeklyCocoonLimit { get; set; }

    [JsonPropertyName("current_reserve_stamina")]
    public int CurrentReserveStamina { get; set; }

    [JsonPropertyName("is_reserve_stamina_full")]
    public bool IsReserveStaminaFull { get; set; }

    [JsonPropertyName("rogue_tourn_weekly_unlocked")]
    public bool RogueTournWeeklyUnlocked { get; set; }

    [JsonPropertyName("rogue_tourn_weekly_max")]
    public int RogueTournWeeklyMax { get; set; }

    [JsonPropertyName("rogue_tourn_weekly_cur")]
    public int RogueTournWeeklyCurrent { get; set; }

    [JsonPropertyName("current_ts")]
    public long CurrentTimestamp { get; set; }

    [JsonPropertyName("rogue_tourn_exp_is_full")]
    public bool RogueTournExpIsFull { get; set; }
}

public class StarRailExpedition
{
    [JsonPropertyName("avatars")]
    public string[]? Avatars { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("remaining_time")]
    public int RemainingTime { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("item_url")]
    public string ItemUrl { get; set; } = string.Empty;

    [JsonPropertyName("finish_ts")]
    public long FinishTimestamp { get; set; }
}

public enum StarRailExpeditionStatus
{
    Ongoing,
    Finished
} 