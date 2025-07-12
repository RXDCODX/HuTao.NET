namespace HuTao.NET.GI;

public class EndPoints
{
    // Common endpoints
    public EndPoint UserStats = new(
        "https://bbs-api-os.hoyoverse.com/game_record/card/wapi/getGameRecordCard",
        true,
        false
    );
    public EndPoint UserAccountInfo = new(
        "https://api-account-os.hoyolab.com/auth/api/getUserAccountInfoByLToken",
        true,
        false
    );
    public EndPoint GetLanguage = new(
        "https://bbs-api-os.hoyolab.com/community/misc/wapi/langs",
        false,
        false
    );
    public EndPoint GetRoles = new(
        "https://api-account-os.hoyolab.com/binding/api/getUserGameRolesByLtoken",
        true,
        false
    );

    // Genshin Impact endpoints
    public EndPoint GenshinStats = new(
        "https://bbs-api-os.hoyolab.com/game_record/genshin/api/index",
        true,
        true
    );
    public EndPoint GenshinDailyNote = new(
        "https://bbs-api-os.hoyolab.com/game_record/genshin/api/dailyNote",
        true,
        true
    );
    public EndPoint GenshinRewardInfo = new(
        "https://sg-hk4e-api.hoyolab.com/event/sol/info?act_id=e202102251931481",
        false,
        false
    );
    public EndPoint GenshinRewardData = new(
        "https://sg-hk4e-api.hoyolab.com/event/sol/home?act_id=e202102251931481",
        false,
        false
    );
    public EndPoint GenshinRewardSign = new(
        "https://sg-hk4e-api.hoyolab.com/event/sol/sign?act_id=e202102251931481",
        true,
        false
    );

    // Honkai Star Rail endpoints
    public EndPoint StarRailStats = new(
        "https://bbs-api-os.hoyolab.com/game_record/hkrpg/api/index",
        true,
        true
    );
    public EndPoint StarRailDailyNote = new(
        "https://bbs-api-os.hoyolab.com/game_record/hkrpg/api/note",
        true,
        true
    );
    public EndPoint StarRailRewardInfo = new(
        "https://sg-public-api.hoyolab.com/event/luna/hkrpg/os/info?act_id=e202303301540311",
        false,
        false
    );
    public EndPoint StarRailRewardData = new(
        "https://sg-public-api.hoyolab.com/event/luna/hkrpg/os/home?act_id=e202303301540311",
        false,
        false
    );
    public EndPoint StarRailRewardSign = new(
        "https://sg-public-api.hoyolab.com/event/luna/hkrpg/os/sign?act_id=e202303301540311",
        true,
        false
    );

    // Zenless Zone Zero endpoints (placeholder for future)
    public EndPoint ZenlessStats = new(
        "https://bbs-api-os.hoyolab.com/game_record/zzz/api/index",
        true,
        true
    );
    public EndPoint ZenlessDailyNote = new(
        "https://bbs-api-os.hoyolab.com/game_record/zzz/api/note",
        true,
        true
    );
}

public class EndPoint(string url, bool isAuth = true, bool isDS = false)
{
    public string Url { get; set; } = url;
    public bool RequireAuth { get; set; } = isAuth;
    public bool RequireDS { get; set; } = isDS;
}
