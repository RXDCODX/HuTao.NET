using HuTao.NET.GI;
using Xunit.Abstractions;

namespace HuTao.NET.Tests;

public class StarRailTest(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public async Task TestStarRailRewardData()
    {
        // Create test cookie (you'll need to provide real credentials)
        var cookie = new Cookie { LToken = "your_ltoken_here", LtUid = "your_ltuid_here" };

        // Using root client instead of direct StarRailClient
        using var client = HuTaoClient.Create(cookie);
        var starRailClient = client.StarRail;

        try
        {
            // Test getting reward data
            var rewardData = await starRailClient.GetStarRailRewardData();

            Assert.NotNull(rewardData);
            Assert.Equal(0, rewardData.retcode);
            Assert.NotNull(rewardData.Data);
            Assert.NotNull(rewardData.Data.Awards);

            testOutputHelper.WriteLine($"Month: {rewardData.Data.Month}");
            testOutputHelper.WriteLine($"Total awards: {rewardData.Data.Awards.Length}");

            // Print first few awards
            for (var i = 0; i < Math.Min(5, rewardData.Data.Awards.Length); i++)
            {
                var award = rewardData.Data.Awards[i];
                testOutputHelper.WriteLine($"Award {i + 1}: {award.Name} x{award.Count}");
            }
        }
        catch (Exception ex)
        {
            testOutputHelper.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }

    [Fact]
    public Task TestStarRailUserCreation()
    {
        // Test UID for Asia server (starts with 8)
        var user = new StarRailUser(800000000);

        Assert.Equal(800000000, user.Uid);
        Assert.Equal("prod_official_asia", user.Server);

        Console.WriteLine($"UID: {user.Uid}");
        Console.WriteLine($"Server: {user.Server}");
        return Task.CompletedTask;
    }

    [Fact]
    public async Task TestStarRailDailyNote()
    {
        // Create test cookie (you'll need to provide real credentials)
        var cookie = new Cookie { LToken = "your_ltoken_here", LtUid = "your_ltuid_here" };

        // Using root client instead of direct StarRailClient
        using var client = HuTaoClient.Create(cookie);
        var starRailClient = client.StarRail;
        var user = new StarRailUser(700378086); // Real UID from user's account

        try
        {
            // Test getting daily note
            var dailyNote = await starRailClient.FetchDailyNote(user);

            Assert.NotNull(dailyNote);
            Assert.Equal(0, dailyNote.retcode);
            Assert.NotNull(dailyNote.Data);

            Console.WriteLine(
                $"Current Stamina: {dailyNote.Data.CurrentStamina}/{dailyNote.Data.MaxStamina}"
            );
            Console.WriteLine(
                $"Stamina Percentage: {StarRailClient.GetStaminaPercentage(dailyNote.Data):F1}%"
            );
            Console.WriteLine($"Is Stamina Full: {StarRailClient.IsStaminaFull(dailyNote.Data)}");
            Console.WriteLine(
                $"Stamina Recovery Time: {StarRailClient.GetStaminaRecoveryTime(dailyNote.Data)}"
            );
            Console.WriteLine(
                $"Stamina Full Time: {StarRailClient.GetStaminaFullTime(dailyNote.Data)}"
            );

            Console.WriteLine(
                $"Expeditions: {dailyNote.Data.AcceptedExpeditionNum}/{dailyNote.Data.TotalExpeditionNum}"
            );
            Console.WriteLine(
                $"Train Score: {dailyNote.Data.CurrentTrainScore}/{dailyNote.Data.MaxTrainScore}"
            );
            Console.WriteLine(
                $"Rogue Score: {dailyNote.Data.CurrentRogueScore}/{dailyNote.Data.MaxRogueScore}"
            );

            if (dailyNote.Data.Expeditions != null)
            {
                foreach (var expedition in dailyNote.Data.Expeditions)
                {
                    Console.WriteLine($"Expedition: {expedition.Name} - {expedition.Status}");
                    Console.WriteLine(
                        $"Remaining Time: {StarRailClient.GetExpeditionRemainingTime(expedition)}"
                    );
                    Console.WriteLine(
                        $"Completion Time: {StarRailClient.GetExpeditionCompletionTime(expedition)}"
                    );
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }
}
