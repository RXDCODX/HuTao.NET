using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using static HuTao.NET.Util.Errors;

namespace HuTao.NET.Tests;

public class UnitTest
{
    private readonly GenshinClient _genshinClient;
    private readonly GenshinUser _user;

    public const string TestLogFileName = "TestLog.log";

    public UnitTest()
    {
        // Используем тестовые данные вместо чтения файла
        ICookie cookie = new CookieV2()
        {
            LtMidV2 = "test_mid",
            LTokenV2 = "test_token",
            LtUidV2 = "test_uid",
        };
        _genshinClient = GenshinClient.Create(cookie, new ClientData() { Language = "ja-jp" });
        _user = new GenshinUser(123456789); // Тестовый UID
    }

    [ModuleInitializer]
    internal static void LogInit()
    {
        //ѓЌѓOѓtѓ@ѓCѓ‹Џ‰Љъ‰»
        if (File.Exists(TestLogFileName))
        {
            File.Delete(TestLogFileName);
        }
        ;
        File.AppendAllText(TestLogFileName, "Unit Test: " + DateTime.Now.ToString() + "\n");
    }

    private static void WriteLog(object obj, string logName)
    {
        var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
        File.AppendAllText(TestLogFileName, "[UnitTest][" + logName + "]\n" + json + "\n");
    }

    [Fact]
    public async void UserStats()
    {
        var res = await _genshinClient.FetchUserStats();

        Assert.NotNull(res.Data);
        WriteLog(res, "FetchUserStats");
    }

    [Fact]
    public async void GetRoles()
    {
        var res = await _genshinClient.GetGameRoles();

        Assert.NotNull(res.Data);
        WriteLog(res, "GetGameRoles");
    }

    [Fact]
    public async void AccountInfo()
    {
        var res = await _genshinClient.GetUserAccountInfoByLToken();

        Assert.Equal("OK", res.Message);
        WriteLog(res, "GetUserAccountInfoByLToken");
    }

    [Fact]
    public async void GetGenshinStats()
    {
        var res = await _genshinClient.FetchGenshinStats(_user);

        Assert.Equal("OK", res.Message);
        WriteLog(res, "FetchGenshinStats");
    }

    [Fact]
    public async void GetNote()
    {
        var res = await _genshinClient.FetchDailyNote(_user);

        Assert.Equal("OK", res.Message);
        WriteLog(res, "FetchDailyNote");
    }

    [Fact]
    public async void GetLangs()
    {
        var res = await GenshinClient.GetAvailableLanguages();

        Assert.Equal("OK", res.Message);
        WriteLog(res, "GetAvailableLanguages");
    }

    [Fact]
    public async void ClaimDailyReward()
    {
        try
        {
            var res = await _genshinClient.ClaimDailyReward();
            Assert.True(res.IsSuccessed);
            WriteLog(res, "ClaimDailyReward");
        }
        catch (DailyRewardAlreadyReceivedException)
        {
            Assert.True(true, "daily reward is already received!");
        }
    }
}
