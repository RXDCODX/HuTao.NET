using static HuTao.NET.GI.Util.Errors;

namespace HuTao.NET.GI.Util;

public class Server
{
    public class Servers
    {
        // Common servers for all games
        public static readonly string ASIA = "os_asia";
        public static readonly string TW_HK_MO = "os_cht";
        public static readonly string AMERICA = "os_usa";
        public static readonly string EUROPE = "os_euro";
        public static readonly string CHINA_CELESTIA = "cn_gf01";
        public static readonly string CHINA_IRUMINSUI = "cn_qd01";

        // Honkai Star Rail specific servers
        public static readonly string STARRAIL_ASIA = "prod_official_asia";
        public static readonly string STARRAIL_AMERICA = "prod_official_usa";
        public static readonly string STARRAIL_EUROPE = "prod_official_eur";
    }

    /// <summary>
    /// Get server for Genshin Impact based on UID
    /// </summary>
    internal static string GetGenshinServer(int uid)
    {
        return uid.ToString()[..1] switch
        {
            "1" => Servers.CHINA_CELESTIA,
            "2" => Servers.CHINA_CELESTIA,
            "5" => Servers.CHINA_IRUMINSUI,
            "6" => Servers.AMERICA,
            "7" => Servers.EUROPE,
            "8" => Servers.ASIA,
            "9" => Servers.TW_HK_MO,
            _ => throw new AccountNotFoundException(
                "Could not identify the server for this Genshin UID."
            ),
        };
    }

    /// <summary>
    /// Get server for Honkai Star Rail based on UID
    /// </summary>
    internal static string GetStarRailServer(int uid)
    {
        return uid.ToString()[..1] switch
        {
            "1" => Servers.CHINA_CELESTIA,
            "2" => Servers.CHINA_CELESTIA,
            "5" => Servers.CHINA_IRUMINSUI,
            "6" => Servers.STARRAIL_AMERICA,
            "7" => Servers.STARRAIL_EUROPE,
            "8" => Servers.STARRAIL_ASIA,
            "9" => Servers.TW_HK_MO,
            _ => throw new AccountNotFoundException(
                "Could not identify the server for this Star Rail UID."
            ),
        };
    }

    /// <summary>
    /// Get server for Zenless Zone Zero based on UID (placeholder for future)
    /// </summary>
    internal static string GetZenlessServer(int uid)
    {
        // TODO: Implement when Zenless Zone Zero is released
        // This will likely follow the same pattern as other games
        return uid.ToString()[..1] switch
        {
            "1" => Servers.CHINA_CELESTIA,
            "2" => Servers.CHINA_CELESTIA,
            "5" => Servers.CHINA_IRUMINSUI,
            "6" => Servers.AMERICA,
            "7" => Servers.EUROPE,
            "8" => Servers.ASIA,
            "9" => Servers.TW_HK_MO,
            _ => throw new AccountNotFoundException(
                "Could not identify the server for this Zenless UID."
            ),
        };
    }
}
