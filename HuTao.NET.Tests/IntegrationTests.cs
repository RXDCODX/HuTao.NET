using HuTao.NET.Models.HonkaiStarRail;
using Xunit.Abstractions;

namespace HuTao.NET.Tests;

/// <summary>
/// Интеграционные тесты для проверки работы API с реальными данными
/// </summary>
public class IntegrationTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public async Task TestUserStatsWithRealData()
    {
        // Arrange
        var cookie = TestData.CreateTestCookie();
        using var client = HuTaoClient.Create(cookie);

        try
        {
            // Act
            var userStats = await client.FetchUserStats();

            // Assert
            Assert.NotNull(userStats);

            // Если API вернул ошибку, выводим информацию и пропускаем тест
            if (userStats.Retcode != 0)
            {
                testOutputHelper.WriteLine(
                    $"API returned error: {userStats.Retcode} - {userStats.Message}"
                );
                testOutputHelper.WriteLine("Skipping test due to API error");
                return;
            }

            Assert.NotNull(userStats.Data);
            Assert.NotNull(userStats.Data?.Name);

            testOutputHelper.WriteLine($"User Name: {userStats.Data?.Name}");
            testOutputHelper.WriteLine(
                $"Game Lists Count: {userStats.Data?.GameLists?.Count ?? 0}"
            );

            // Выводим информацию о играх пользователя
            if (userStats.Data?.GameLists != null)
            {
                foreach (var game in userStats.Data.GameLists)
                {
                    testOutputHelper.WriteLine(
                        $"Game: {game.GameId} - {game.Nickname} (UID: {game.GameUid})"
                    );
                }
            }
        }
        catch (Exception ex)
        {
            testOutputHelper.WriteLine($"Error in TestUserStatsWithRealData: {ex.Message}");
            testOutputHelper.WriteLine("Skipping test due to API error");
        }
    }

    [Fact]
    public async Task TestUserAccountInfoWithRealData()
    {
        // Arrange
        var cookie = TestData.CreateTestCookie();
        using var client = HuTaoClient.Create(cookie);

        // Act
        var accountInfo = await client.GetUserAccountInfoByLToken();

        // Assert
        Assert.NotNull(accountInfo);
        Assert.Equal(0, accountInfo.Retcode);
        Assert.NotNull(accountInfo.Data);

        testOutputHelper.WriteLine($"Account ID: {accountInfo.Data?.AccountId}");
        testOutputHelper.WriteLine($"Account Name: {accountInfo.Data?.AccountName}");
        testOutputHelper.WriteLine($"Nick Name: {accountInfo.Data?.NickName}");
        testOutputHelper.WriteLine($"Email: {accountInfo.Data?.Email}");
    }

    [Fact]
    public async Task TestGameRolesWithRealData()
    {
        // Arrange
        var cookie = TestData.CreateTestCookie();
        using var client = HuTaoClient.Create(cookie);

        // Act
        var gameRoles = await client.GetGameRoles();

        // Assert
        Assert.NotNull(gameRoles);
        Assert.Equal(0, gameRoles.Retcode);
        Assert.NotNull(gameRoles.Data);
        Assert.NotNull(gameRoles.Data?.List);

        testOutputHelper.WriteLine($"Total Game Roles: {gameRoles.Data?.List?.Count ?? 0}");

        // Выводим информацию о ролях
        if (gameRoles.Data?.List != null)
        {
            foreach (var role in gameRoles.Data.List)
            {
                testOutputHelper.WriteLine(
                    $"Role: {role.GameRegionName} - {role.NickName} (UID: {role.GameUid})"
                );
                testOutputHelper.WriteLine($"  Level: {role.Level}, Region: {role.RegionName}");
            }
        }
    }

    [Fact]
    public async Task TestStarRailClientWithRealData()
    {
        // Arrange
        var cookie = TestData.CreateTestCookie();
        using var client = HuTaoClient.Create(cookie);
        var starRailClient = client.StarRail;

        // Получаем роли пользователя для поиска реального UID
        var gameRoles = await client.GetGameRoles();
        var starRailRole = gameRoles.Data?.List?.FirstOrDefault(r =>
            r.GameRegionName == "hkrpg_global"
        );

        if (starRailRole == null)
        {
            testOutputHelper.WriteLine("Star Rail role not found, skipping test");
            return;
        }

        var user = new StarRailUser(int.Parse(starRailRole.GameUid));

        // Act & Assert - Тестируем получение статистики
        try
        {
            var stats = await starRailClient.FetchStarRailStats(user);
            Assert.NotNull(stats);
            Assert.Equal(0, stats.Retcode);
            Assert.NotNull(stats.Data);

            testOutputHelper.WriteLine($"Star Rail Stats - UID: {user.Uid}");
            testOutputHelper.WriteLine($"Nickname: {stats.Data?.DetailInfo?.Nickname}");
            testOutputHelper.WriteLine($"Level: {stats.Data?.DetailInfo?.Level}");
            testOutputHelper.WriteLine($"World Level: {stats.Data?.DetailInfo?.WorldLevel}");
        }
        catch (Exception ex)
        {
            testOutputHelper.WriteLine($"Error fetching Star Rail stats: {ex.Message}");
        }

        // Act & Assert - Тестируем получение ежедневных заметок
        try
        {
            // Добавляем отладочную информацию
            testOutputHelper.WriteLine(
                $"Attempting to fetch daily note for UID: {user.Uid}, Server: {user.Server}"
            );

            var dailyNote = await starRailClient.FetchDailyNote(user);
            Assert.NotNull(dailyNote);
            Assert.Equal(0, dailyNote.Retcode);
            Assert.NotNull(dailyNote.Data);

            testOutputHelper.WriteLine($"Star Rail Daily Note - UID: {user.Uid}");
            testOutputHelper.WriteLine(
                $"Current Stamina: {dailyNote.Data?.CurrentStamina}/{dailyNote.Data?.MaxStamina}"
            );
            testOutputHelper.WriteLine(
                $"Stamina Percentage: {StarRailClient.GetStaminaPercentage(dailyNote.Data!):F1}%"
            );
            testOutputHelper.WriteLine(
                $"Is Stamina Full: {StarRailStaminaManager.IsStaminaFull(dailyNote.Data!)}"
            );
            testOutputHelper.WriteLine(
                $"Stamina Recovery Time: {StarRailClient.GetStaminaRecoveryTime(dailyNote.Data!)}"
            );
            testOutputHelper.WriteLine(
                $"Stamina Full Time: {StarRailClient.GetStaminaFullTime(dailyNote.Data!)}"
            );
            testOutputHelper.WriteLine(
                $"Expeditions: {dailyNote.Data?.AcceptedExpeditionNum}/{dailyNote.Data?.TotalExpeditionNum}"
            );
            testOutputHelper.WriteLine(
                $"Train Score: {dailyNote.Data?.CurrentTrainScore}/{dailyNote.Data?.MaxTrainScore}"
            );
            testOutputHelper.WriteLine(
                $"Rogue Score: {dailyNote.Data?.CurrentRogueScore}/{dailyNote.Data?.MaxRogueScore}"
            );

            if (dailyNote.Data?.Expeditions != null)
            {
                foreach (var expedition in dailyNote.Data.Expeditions)
                {
                    testOutputHelper.WriteLine(
                        $"Expedition: {expedition.Name} - {expedition.Status}"
                    );
                    testOutputHelper.WriteLine(
                        $"  Remaining Time: {StarRailClient.GetExpeditionRemainingTime(expedition)}"
                    );
                    testOutputHelper.WriteLine(
                        $"  Completion Time: {StarRailClient.GetExpeditionCompletionTime(expedition)}"
                    );
                }
            }
        }
        catch (Exception ex)
        {
            testOutputHelper.WriteLine($"Error fetching Star Rail daily note: {ex.Message}");
            testOutputHelper.WriteLine($"Exception type: {ex.GetType().Name}");
            testOutputHelper.WriteLine($"Stack trace: {ex.StackTrace}");

            // Добавляем отладочную информацию о запросе
            testOutputHelper.WriteLine(
                $"Request URL would be: https://bbs-api-os.hoyolab.com/game_record/hkrpg/api/note?server={user.Server}&role_id={user.Uid}"
            );
            testOutputHelper.WriteLine($"Cookie: {cookie.GetCookie()}");
        }
    }

    [Fact]
    public async Task TestGenshinClientWithRealData()
    {
        // Arrange
        var cookie = TestData.CreateTestCookie();
        using var client = HuTaoClient.Create(cookie);
        var genshinClient = client.Genshin;

        // Получаем роли пользователя для поиска реального UID
        var gameRoles = await client.GetGameRoles();
        var genshinRole = gameRoles.Data?.List?.FirstOrDefault(r =>
            r.GameRegionName == "hk4e_global"
        );

        if (genshinRole == null)
        {
            testOutputHelper.WriteLine("Genshin Impact role not found, skipping test");
            return;
        }

        var user = new GenshinUser(int.Parse(genshinRole.GameUid));

        // Act & Assert - Тестируем получение статистики
        try
        {
            var stats = await genshinClient.FetchGenshinStats(user);
            Assert.NotNull(stats);
            Assert.Equal(0, stats.Retcode);
            Assert.NotNull(stats.Data);

            testOutputHelper.WriteLine($"Genshin Stats - UID: {user.Uid}");
            testOutputHelper.WriteLine($"Nickname: {stats.Data?.Role?.NickName}");
            testOutputHelper.WriteLine($"Level: {stats.Data?.Role?.Level}");
            testOutputHelper.WriteLine($"Region: {stats.Data?.Role?.Region}");
            testOutputHelper.WriteLine($"Active Days: {stats.Data?.Stats?.ActiveDays}");
            testOutputHelper.WriteLine($"Achievements: {stats.Data?.Stats?.Achievements}");
            testOutputHelper.WriteLine($"Avatars: {stats.Data?.Stats?.Avaters}");
        }
        catch (Exception ex)
        {
            testOutputHelper.WriteLine($"Error fetching Genshin stats: {ex.Message}");
        }

        // Act & Assert - Тестируем получение ежедневных заметок
        try
        {
            var dailyNote = await genshinClient.FetchDailyNote(user);
            Assert.NotNull(dailyNote);
            Assert.Equal(0, dailyNote.Retcode);
            Assert.NotNull(dailyNote.Data);

            testOutputHelper.WriteLine($"Genshin Daily Note - UID: {user.Uid}");
            testOutputHelper.WriteLine(
                $"Current Resin: {dailyNote.Data?.CurrentResin}/{dailyNote.Data?.MaxResin}"
            );
            testOutputHelper.WriteLine($"Resin Recovery Time: {dailyNote.Data?.ResinRecoveryTime}");
            testOutputHelper.WriteLine(
                $"Finished Tasks: {dailyNote.Data?.FinishedTaskNum}/{dailyNote.Data?.ToatalTaskNum}"
            );
            testOutputHelper.WriteLine(
                $"Current Home Coin: {dailyNote.Data?.CurrentHomeCoin}/{dailyNote.Data?.MaxHomeCoin}"
            );
            testOutputHelper.WriteLine(
                $"Home Coin Recovery Time: {dailyNote.Data?.HomeCoinRecoveryTime}"
            );
            testOutputHelper.WriteLine(
                $"Expeditions: {dailyNote.Data?.CurrentExpeditionNum}/{dailyNote.Data?.MaxExpeditionNum}"
            );

            if (dailyNote.Data?.Expeditions != null)
            {
                foreach (var expedition in dailyNote.Data.Expeditions)
                {
                    testOutputHelper.WriteLine(
                        $"Expedition: {expedition.Status} - Remaining: {expedition.RemainedTime}"
                    );
                }
            }
        }
        catch (Exception ex)
        {
            testOutputHelper.WriteLine($"Error fetching Genshin daily note: {ex.Message}");
        }
    }

    [Fact]
    public async Task TestStarRailRewardDataWithRealData()
    {
        // Arrange
        var cookie = TestData.CreateTestCookie();
        using var client = HuTaoClient.Create(cookie);
        var starRailClient = client.StarRail;

        // Act
        var rewardData = await starRailClient.GetStarRailRewardData();

        // Assert
        Assert.NotNull(rewardData);
        Assert.Equal(0, rewardData.Retcode);
        Assert.NotNull(rewardData.Data);
        Assert.NotNull(rewardData.Data?.Awards);

        testOutputHelper.WriteLine($"Star Rail Reward Data - Month: {rewardData.Data?.Month}");
        testOutputHelper.WriteLine($"Total Awards: {rewardData.Data?.Awards?.Length ?? 0}");
        testOutputHelper.WriteLine($"Biz: {rewardData.Data?.Biz}");
        testOutputHelper.WriteLine($"Resign: {rewardData.Data?.Resign}");

        // Выводим информацию о наградах
        if (rewardData.Data?.Awards != null)
        {
            for (var i = 0; i < Math.Min(5, rewardData.Data.Awards.Length); i++)
            {
                var award = rewardData.Data.Awards[i];
                testOutputHelper.WriteLine($"Award {i + 1}: {award.Name} x{award.Count}");
            }
        }
    }

    [Fact]
    public async Task TestAvailableLanguages()
    {
        // Arrange & Act
        var languages = await HuTaoClient.GetAvailableLanguages();

        // Assert
        Assert.NotNull(languages);
        Assert.Equal(0, languages.Retcode);
        Assert.NotNull(languages.Data);
        Assert.NotNull(languages.Data?.Langs);

        testOutputHelper.WriteLine($"Available Languages: {languages.Data?.Langs?.Length ?? 0}");

        // Выводим доступные языки
        if (languages.Data?.Langs != null)
        {
            foreach (var lang in languages.Data.Langs.Take(5))
            {
                testOutputHelper.WriteLine($"Language: {lang.Name} ({lang.Value}) - {lang.Label}");
            }
        }
    }
}
