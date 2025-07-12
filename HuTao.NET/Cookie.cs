using System.Text.Json.Serialization;

namespace HuTao.NET.GI;

public interface ICookie
{
    public string GetCookie();
    public string GetHoyoUid();
}

public class Cookie : ICookie
{
    [JsonPropertyName("ltoken")]
    public string LToken { get; set; } = string.Empty;
    [JsonPropertyName("ltuid")]
    public string LtUid { get; set; } = string.Empty;

    public string GetCookie()
    {
        return $"ltoken={LToken}; ltuid={LtUid}";
    }

    public string GetHoyoUid()
    {
        return LtUid;
    }
}

public class CookieV2 : ICookie
{
    [JsonPropertyName("ltoken_v2")]
    public string LTokenV2 { get; set; } = string.Empty;
    [JsonPropertyName("ltmid_v2")]
    public string LtMidV2 { get; set; } = string.Empty;

    [JsonPropertyName("ltuid_v2")]
    public string LtUidV2 { get; set; } = string.Empty;

    public string GetCookie()
    {
        return $"ltoken_v2={LTokenV2}; ltmid_v2={LtMidV2}; ltuid_v2={LtUidV2}";
    }

    public string GetHoyoUid()
    {
        return LtUidV2;
    }
}

public class RawCookie(string cookie, string hoyolabUid) : ICookie
{
    public string GetCookie()
    {
        return cookie;
    }

    public string GetHoyoUid()
    {
        return hoyolabUid;
    }
}
