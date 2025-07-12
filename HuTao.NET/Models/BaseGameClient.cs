using System.Text.Json.Nodes;
using HuTao.NET.GI.Models.GenshinImpact;
using HuTao.NET.GI.Models.HoYoLab;

namespace HuTao.NET.GI.Models;

/// <summary>
/// Base class for all game clients that handle HoyoLab WebAPI
/// </summary>
public abstract class BaseGameClient(ICookie cookie, ClientData data) : IGameClient
{
    protected readonly ICookie _cookie = cookie;
    protected readonly ClientData _clientData = data;

    protected BaseGameClient(ICookie cookie)
        : this(cookie, new ClientData()) { }

    /// <summary>
    /// Get user statistics
    /// </summary>
    public virtual async Task<UserStats> FetchUserStats(string? uid = null)
    {
        var targetUid = uid ?? _cookie.GetHoyoUid();
        var url = _clientData.EndPoints.UserStats.Url + $"?uid={targetUid}";
        return await new Wrapper<UserStats>(_clientData).FetchData(url, _cookie);
    }

    /// <summary>
    /// Get user account information by LToken
    /// </summary>
    public virtual async Task<UserAccountInfo> GetUserAccountInfoByLToken()
    {
        var url = _clientData.EndPoints.UserAccountInfo.Url;
        return await new Wrapper<UserAccountInfo>(_clientData).FetchData(url, _cookie);
    }

    /// <summary>
    /// Get game roles for the user
    /// </summary>
    public virtual async Task<GameRoles> GetGameRoles(bool isGameOnly = true, string region = "")
    {
        var url = _clientData.EndPoints.GetRoles.Url;
        url += isGameOnly ? $"&game_biz={GetGameBiz()}" : "";
        url += !string.IsNullOrEmpty(region) ? $"&region={region}" : "";
        return await new Wrapper<GameRoles>(_clientData).FetchData(url, _cookie);
    }

    /// <summary>
    /// Get available languages
    /// </summary>
    public static async Task<Languages> GetAvailableLanguages()
    {
        var data = new ClientData();
        return await new Wrapper<Languages>(data).FetchData(data.EndPoints.GetLanguage.Url);
    }

    /// <summary>
    /// Get game business identifier for API calls
    /// </summary>
    protected abstract string GetGameBiz();

    /// <summary>
    /// Get game ID for role filtering
    /// </summary>
    protected abstract int GetGameId();

    /// <summary>
    /// Fetch dynamic API data
    /// </summary>
    protected async Task<JsonNode> FetchDynamicApi(string url, bool isPost = false)
    {
        HttpResponseMessage res;
        if (!isPost)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, url);

            req.Headers.Add("x-rpc-app_version", "1.5.0");
            req.Headers.Add("x-rpc-client_type", "5");
            req.Headers.Add("x-rpc-language", _clientData.Language);
            req.Headers.Add("user-agent", _clientData.UserAgent);
            req.Headers.Add("Cookie", _cookie.GetCookie());

            // Add Star Rail specific headers if needed
            if (url.Contains("luna/hkrpg"))
            {
                req.Headers.Add("x-rpc-lrsag", "");
                req.Headers.Add("x-rpc-signgame", "hkrpg");
            }

            res = await _clientData.HttpClient.SendAsync(req);
        }
        else
        {
            HttpContent req = new StringContent("");

            req.Headers.Add("x-rpc-app_version", "1.5.0");
            req.Headers.Add("x-rpc-client_type", "5");
            req.Headers.Add("x-rpc-language", _clientData.Language);
            _clientData.HttpClient.DefaultRequestHeaders.Add("User-Agent", _clientData.UserAgent);
            req.Headers.Add("Cookie", _cookie.GetCookie());

            // Add Star Rail specific headers if needed
            if (url.Contains("luna/hkrpg"))
            {
                req.Headers.Add("x-rpc-lrsag", "");
                req.Headers.Add("x-rpc-signgame", "hkrpg");
            }

            res = await _clientData.HttpClient.PostAsync(url, req);
        }

        var jsonString = await res.Content.ReadAsStringAsync();
        return JsonNode.Parse(jsonString) ?? throw new NullReferenceException();
    }
}
