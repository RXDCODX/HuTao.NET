using HuTao.NET.Auth;
using HuTao.NET.GI;

namespace HuTao.NET.Tests;

public class AuthTest
{
    /// <summary>
    /// 自動化できないテスト
    /// </summary>
    //[Fact]
    internal static async void BrowserAuth()
    {
        var login = new BrowserLogin();
        login.OpenBrowser();
        await Task.Delay(30000); //30秒後に取得する
        var cookie = login.GetCookie();

        Assert.NotNull(cookie);

        login.CloseBrowser();
    }

    [Fact]
    public void Parse()
    {
        var str =
            "mi18nLang=ja-jp; _MHYUUID=00000000-xxxx-0000-xxxx-000000000000; G_ENABLED_IDPS=google; ltoken=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx; ltuid=00000000; DEVICEFP_SEED_ID=xxxxxxxxxxxxxxxx; DEVICEFP_SEED_TIME=0000000000000; DEVICEFP=0000000000000; G_AUTHUSER_H=0";
        var cookie = Parser.CookieParser(str);

        Assert.False(string.IsNullOrEmpty(cookie.LtUid) || string.IsNullOrEmpty(cookie.LToken));
    }
}
