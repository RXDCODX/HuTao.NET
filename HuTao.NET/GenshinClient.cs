using HuTao.NET.Models;
using HuTao.NET.Models.GenshinImpact;
using HuTao.NET.Util;

namespace HuTao.NET;

/// <summary>
/// Client data configuration for HoyoLab API
/// </summary>
public class ClientData
{
    public string UserAgent { get; set; } =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36";
    public EndPoints EndPoints { get; set; } = new();
    public string Language { get; set; } = "en-us";
    public HttpClient HttpClient { get; set; } = new();
}

/// <summary>
/// Genshin Impact client for HoyoLab WebAPI
/// </summary>
public class GenshinClient : BaseGameClient
{
    internal GenshinClient(ICookie cookie)
        : base(cookie) { }

    internal GenshinClient(ICookie cookie, ClientData data)
        : base(cookie, data) { }

    /// <summary>
    /// Get Genshin Impact statistics
    /// </summary>
    public async Task<GenshinStats> FetchGenshinStats(GenshinUser user)
    {
        var url =
            _clientData.EndPoints.GenshinStats.Url + $"?server={user.Server}&role_id={user.Uid}";
        return await new Wrapper<GenshinStats>(_clientData).FetchData(url, _cookie, true);
    }

    /// <summary>
    /// Get daily note for Genshin Impact
    /// </summary>
    public async Task<DailyNote> FetchDailyNote(GenshinUser user)
    {
        var url =
            _clientData.EndPoints.GenshinDailyNote.Url
            + $"?server={user.Server}&role_id={user.Uid}";
        return await new Wrapper<DailyNote>(_clientData).FetchData(url, _cookie, true);
    }

    /// <summary>
    /// Claim daily reward for Genshin Impact
    /// </summary>
    public async Task<RewardData> ClaimDailyReward()
    {
        var data = new RewardData();

        var infoUrl = _clientData.EndPoints.GenshinRewardInfo.Url + "&lang=" + _clientData.Language;
        var homeUrl = _clientData.EndPoints.GenshinRewardData.Url + "&lang=" + _clientData.Language;
        var claimUrl =
            _clientData.EndPoints.GenshinRewardSign.Url + "&lang=" + _clientData.Language;

        // Get total sign days for home calculation
        var info = await FetchDynamicApi(infoUrl, false);
        var days =
            info?["data"]?["total_sign_day"]?.GetValue<int>() ?? throw new NullReferenceException();
        days--; // Subtract 1 from total days

        // Get item name and amount from home
        var home = await FetchDynamicApi(homeUrl, false);
        var name =
            home?["data"]?["awards"]?[days]?["name"]?.GetValue<string>()
            ?? throw new NullReferenceException();
        var amount =
            home?["data"]?["awards"]?[days]?["cnt"]?.GetValue<int>()
            ?? throw new NullReferenceException();

        data.RewardName = name;
        data.Amount = amount;

        // Claim reward
        var sign = await FetchDynamicApi(claimUrl, true);
        var code = sign?["retcode"]?.GetValue<int>() ?? throw new NullReferenceException();
        if (code == 0)
        {
            data.IsSuccessed = true;
        }
        else if (code == -5003)
        {
            throw new Errors.DailyRewardAlreadyReceivedException();
        }
        else if (sign?["data"]?["gt_result"]?["is_risk"]?.GetValue<string>() == "true")
        {
            throw new Errors.HoyoLabCaptchaBlockException();
        }
        else
        {
            var message =
                sign?["message"]?.GetValue<string>() ?? throw new NullReferenceException();
            throw new Errors.HoyoLabApiBadRequestException(message, code);
        }

        return data;
    }

    protected override string GetGameBiz()
    {
        return "hk4e_global";
    }

    protected override int GetGameId()
    {
        return 2; // Genshin Impact game ID
    }

    public static GenshinClient Create(ICookie cookie, ClientData? data = null)
    {
        return data == null ? new GenshinClient(cookie) : new GenshinClient(cookie, data);
    }
}
