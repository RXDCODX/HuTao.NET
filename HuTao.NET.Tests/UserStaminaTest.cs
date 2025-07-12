using HuTao.NET.Models.HonkaiStarRail;

namespace HuTao.NET.Tests;

public class UserStaminaTest
{
    [Fact]
    public async Task GetUserStaminaInfo()
    {
        // Create cookies with provided data
        var cookie = new CookieV2
        {
            LTokenV2 =
                "v2_CAISDGNpZWJod3pwcnBxOBokYjExZGNlOWQtNTczYS00MGQzLThiMmQtZWI3OGQ1MDE2YTFmILKZyLkGKLmNgvsGMMHN2ZMBQgxoa3JwZ19nbG9iYWw.sgwyZwAAAAAB.MEQCIALHwNYppKlMXrPZhvZRaWzkU7iNSY89gTI1ljDoH0rEAiATCp4AqmPGTzS0UIVDhwgRPYJEznKg65csAcOrxmVyQA",
            LtMidV2 = "1vw187ejtb_hy",
            LtUidV2 = "309749441",
        };

        // Create client
        using var client = HuTaoClient.Create(cookie);
        var starRailClient = client.StarRail;

        try
        {
            // Get user roles
            var gameRoles = await client.GetGameRoles();
            var starRailRole = gameRoles.Data?.List?.FirstOrDefault(r =>
                r.GameRegionName == "hkrpg_global"
            );

            if (starRailRole == null)
            {
                Console.WriteLine("Star Rail role not found!");
                return;
            }

            var user = new StarRailUser(int.Parse(starRailRole.GameUid));
            Console.WriteLine($"UID: {user.Uid}");
            Console.WriteLine($"Server: {user.Server}");

            // Get daily note data
            var dailyNote = await starRailClient.FetchDailyNote(user);

            if (dailyNote?.Data == null)
            {
                Console.WriteLine("Failed to get stamina data!");
                return;
            }

            var data = dailyNote.Data;

            // Display stamina information
            Console.WriteLine("\n=== STAMINA INFORMATION ===");
            Console.WriteLine($"Current Stamina: {data.CurrentStamina}/{data.MaxStamina}");
            Console.WriteLine(
                $"Stamina Percentage: {StarRailStaminaManager.GetStaminaPercentage(data):P1}"
            );
            Console.WriteLine(
                $"Is Stamina Full: {(StarRailStaminaManager.IsStaminaFull(data) ? "Yes" : "No")}"
            );

            // Recovery time
            var recoveryTime = StarRailStaminaManager.GetStaminaRecoveryTime(data);
            var fullTime = StarRailStaminaManager.GetStaminaFullTime(data);
            Console.WriteLine(
                $"Time to Full Recovery: {recoveryTime.Hours:D2}:{recoveryTime.Minutes:D2}:{recoveryTime.Seconds:D2}"
            );
            Console.WriteLine(
                $"Stamina will be full at: {fullTime:HH:mm:ss} ({fullTime:dd.MM.yyyy})"
            );

            // Next stamina point
            var nextPointTime = StarRailStaminaManager.GetNextStaminaPointTime(data);
            var timeToNextPoint = StarRailStaminaManager.GetTimeToNextStaminaPoint(data);
            Console.WriteLine($"Next Stamina Point at: {nextPointTime:HH:mm:ss}");
            Console.WriteLine(
                $"Time to Next Point: {timeToNextPoint.Minutes:D2}:{timeToNextPoint.Seconds:D2}"
            );

            // Forecasts
            var staminaIn1Hour = StarRailStaminaManager.GetStaminaAtTime(
                data,
                TimeSpan.FromHours(1)
            );
            var staminaIn3Hours = StarRailStaminaManager.GetStaminaAtTime(
                data,
                TimeSpan.FromHours(3)
            );
            var staminaIn6Hours = StarRailStaminaManager.GetStaminaAtTime(
                data,
                TimeSpan.FromHours(6)
            );

            Console.WriteLine($"\n=== FORECASTS ===");
            Console.WriteLine($"Stamina in 1 hour: {staminaIn1Hour}");
            Console.WriteLine($"Stamina in 3 hours: {staminaIn3Hours}");
            Console.WriteLine($"Stamina in 6 hours: {staminaIn6Hours}");

            // Recommendations
            Console.WriteLine($"\n=== RECOMMENDATIONS ===");
            Console.WriteLine("• Use stamina efficiently");

            // Additional information
            Console.WriteLine($"\n=== ADDITIONAL INFORMATION ===");
            Console.WriteLine(
                $"Expeditions: {data.AcceptedExpeditionNum}/{data.TotalExpeditionNum}"
            );
            Console.WriteLine($"Training Score: {data.CurrentTrainScore}/{data.MaxTrainScore}");
            Console.WriteLine($"Rogue Score: {data.CurrentRogueScore}/{data.MaxRogueScore}");

            if (data.Expeditions != null && data.Expeditions.Length > 0)
            {
                Console.WriteLine($"\n=== EXPEDITIONS ===");
                foreach (var expedition in data.Expeditions)
                {
                    var remainingTime = StarRailClient.GetExpeditionRemainingTime(expedition);
                    var completionTime = StarRailClient.GetExpeditionCompletionTime(expedition);
                    Console.WriteLine($"• {expedition.Name}: {expedition.Status}");
                    Console.WriteLine($"  Remaining Time: {remainingTime}");
                    Console.WriteLine($"  Will Complete at: {completionTime:HH:mm:ss}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting data: {ex.Message}");
            Console.WriteLine($"Error type: {ex.GetType().Name}");
        }
    }
}
