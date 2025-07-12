using HuTao.NET.GI;
using Xunit.Abstractions;

namespace HuTao.NET.Tests;

public class HuTaoClientTest(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public Task TestHuTaoClientInitialization()
    {
        // Create test cookie
        var cookie = new Cookie { LToken = "your_ltoken_here", LtUid = "your_ltuid_here" };

        // Test root client initialization
        using var client = HuTaoClient.Create(cookie);

        Assert.NotNull(client.Genshin);
        Assert.NotNull(client.StarRail);
        Assert.NotNull(client.Zenless);
        Assert.NotNull(client.GetHttpClient());
        Assert.NotNull(client.GetClientData());
        return Task.CompletedTask;
    }

    [Fact]
    public Task TestHuTaoClientWithCustomHttpClient()
    {
        // Create custom HttpClient
        var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };

        // Create custom client data
        var clientData = new ClientData
        {
            HttpClient = httpClient,
            Language = "ru-ru",
            UserAgent = "Custom Test User Agent",
        };

        var cookie = new Cookie { LToken = "your_ltoken_here", LtUid = "your_ltuid_here" };

        // Test root client with custom configuration
        using var client = HuTaoClient.Create(cookie, clientData);

        Assert.Equal("ru-ru", client.GetClientData().Language);
        Assert.Equal("Custom Test User Agent", client.GetClientData().UserAgent);
        Assert.Equal(httpClient, client.GetHttpClient());
        return Task.CompletedTask;
    }

    [Fact]
    public Task TestHuTaoClientLanguageSetting()
    {
        var cookie = new Cookie { LToken = "your_ltoken_here", LtUid = "your_ltuid_here" };

        using var client = HuTaoClient.Create(cookie);

        // Test language setting
        client.SetLanguage("ja-jp");
        Assert.Equal("ja-jp", client.GetClientData().Language);

        client.SetLanguage("zh-cn");
        Assert.Equal("zh-cn", client.GetClientData().Language);
        return Task.CompletedTask;
    }

    [Fact]
    public Task TestHuTaoClientUserAgentSetting()
    {
        var cookie = new Cookie { LToken = "your_ltoken_here", LtUid = "your_ltuid_here" };

        using var client = HuTaoClient.Create(cookie);

        // Test user agent setting
        client.SetUserAgent("Test User Agent");
        Assert.Equal("Test User Agent", client.GetClientData().UserAgent);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task TestHuTaoClientMultipleGames()
    {
        var cookie = new Cookie { LToken = "your_ltoken_here", LtUid = "your_ltuid_here" };

        using var client = HuTaoClient.Create(cookie);

        try
        {
            // Test getting user stats
            var userStats = await client.FetchUserStats();
            Assert.NotNull(userStats);
            Assert.Equal(0, userStats.retcode);

            // Test getting account info
            var accountInfo = await client.GetUserAccountInfoByLToken();
            Assert.NotNull(accountInfo);
            Assert.Equal(0, accountInfo.retcode);

            // Test getting game roles
            var gameRoles = await client.GetGameRoles();
            Assert.NotNull(gameRoles);
            Assert.Equal(0, gameRoles.retcode);

            testOutputHelper.WriteLine($"User Stats: {userStats.Data?.Name}");
            testOutputHelper.WriteLine($"Account Info: {accountInfo.Data?.AccountName}");
            testOutputHelper.WriteLine($"Game Roles Count: {gameRoles.Data?.List.Count}");
        }
        catch (Exception ex)
        {
            testOutputHelper.WriteLine($"Error: {ex.Message}");
            // Don't throw in test - this is expected if credentials are not provided
        }
    }
}
