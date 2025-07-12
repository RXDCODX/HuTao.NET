using System.Text.Json.Serialization;
using HuTao.NET.Models.GenshinImpact;

namespace HuTao.NET.Models.HonkaiStarRail;

public class StarRailStats : IHoyoLab
{
    public int Retcode { get; set; }
    public string? Message { get; set; }

    [JsonPropertyName("data")]
    public StarRailStatsData? Data { get; set; }
}

public class StarRailStatsData
{
    [JsonPropertyName("detailInfo")]
    public DetailInfo? DetailInfo { get; set; }

    [JsonPropertyName("avatarDetailList")]
    public AvatarDetail[]? AvatarDetailList { get; set; }

    [JsonPropertyName("worldDetailList")]
    public WorldDetail[]? WorldDetailList { get; set; }
}

public class DetailInfo
{
    [JsonPropertyName("uid")]
    public string Uid { get; set; } = string.Empty;

    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = string.Empty;

    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("worldLevel")]
    public int WorldLevel { get; set; }

    [JsonPropertyName("friendCount")]
    public int FriendCount { get; set; }

    [JsonPropertyName("avatarDetailList")]
    public AvatarDetail[]? AvatarDetailList { get; set; }

    [JsonPropertyName("worldDetailList")]
    public WorldDetail[]? WorldDetailList { get; set; }
}

public class AvatarDetail
{
    [JsonPropertyName("avatarId")]
    public int AvatarId { get; set; }

    [JsonPropertyName("avatarName")]
    public string AvatarName { get; set; } = string.Empty;

    [JsonPropertyName("avatarLevel")]
    public int AvatarLevel { get; set; }

    [JsonPropertyName("avatarElement")]
    public string AvatarElement { get; set; } = string.Empty;

    [JsonPropertyName("avatarRarity")]
    public int AvatarRarity { get; set; }

    [JsonPropertyName("avatarPromotion")]
    public int AvatarPromotion { get; set; }

    [JsonPropertyName("avatarSkillList")]
    public AvatarSkill[]? AvatarSkillList { get; set; }

    [JsonPropertyName("avatarRelicList")]
    public AvatarRelic[]? AvatarRelicList { get; set; }
}

public class AvatarSkill
{
    [JsonPropertyName("skillId")]
    public int SkillId { get; set; }

    [JsonPropertyName("skillName")]
    public string SkillName { get; set; } = string.Empty;

    [JsonPropertyName("skillLevel")]
    public int SkillLevel { get; set; }
}

public class AvatarRelic
{
    [JsonPropertyName("relicId")]
    public int RelicId { get; set; }

    [JsonPropertyName("relicName")]
    public string RelicName { get; set; } = string.Empty;

    [JsonPropertyName("relicLevel")]
    public int RelicLevel { get; set; }

    [JsonPropertyName("relicRarity")]
    public int RelicRarity { get; set; }
}

public class WorldDetail
{
    [JsonPropertyName("worldId")]
    public int WorldId { get; set; }

    [JsonPropertyName("worldName")]
    public string WorldName { get; set; } = string.Empty;

    [JsonPropertyName("worldLevel")]
    public int WorldLevel { get; set; }

    [JsonPropertyName("worldProgress")]
    public int WorldProgress { get; set; }
}
