using HuTao.NET.GI;
using HuTao.NET.GI.Models.HonkaiStarRail;

namespace HuTao.NET.Tests;

public class UserStaminaTest
{
    [Fact]
    public async Task GetUserStaminaInfo()
    {
        // Создаем куки с предоставленными данными
        var cookie = new CookieV2
        {
            LTokenV2 = "v2_CAISDGNpZWJod3pwcnBxOBokYjExZGNlOWQtNTczYS00MGQzLThiMmQtZWI3OGQ1MDE2YTFmILKZyLkGKLmNgvsGMMHN2ZMBQgxoa3JwZ19nbG9iYWw.sgwyZwAAAAAB.MEQCIALHwNYppKlMXrPZhvZRaWzkU7iNSY89gTI1ljDoH0rEAiATCp4AqmPGTzS0UIVDhwgRPYJEznKg65csAcOrxmVyQA",
            LtMidV2 = "1vw187ejtb_hy",
            LtUidV2 = "309749441"
        };

        // Создаем клиент
        using var client = HuTaoClient.Create(cookie);
        var starRailClient = client.StarRail;

        try
        {
            // Получаем роли пользователя
            var gameRoles = await client.GetGameRoles();
            var starRailRole = gameRoles.Data?.List?.FirstOrDefault(r => r.GameRegionName == "hkrpg_global");

            if (starRailRole == null)
            {
                Console.WriteLine("Star Rail роль не найдена!");
                return;
            }

            var user = new StarRailUser(int.Parse(starRailRole.GameUid));
            Console.WriteLine($"UID: {user.Uid}");
            Console.WriteLine($"Сервер: {user.Server}");

            // Получаем данные о ежедневной заметке
            var dailyNote = await starRailClient.FetchDailyNote(user);

            if (dailyNote?.Data == null)
            {
                Console.WriteLine("Не удалось получить данные о энергии!");
                return;
            }

            var data = dailyNote.Data;

            // Выводим информацию о энергии
            Console.WriteLine("\n=== ИНФОРМАЦИЯ О ЭНЕРГИИ ===");
            Console.WriteLine($"Текущая энергия: {data.CurrentStamina}/{data.MaxStamina}");
            Console.WriteLine($"Процент заполнения: {StarRailStaminaManager.GetStaminaPercentage(data):P1}");
            Console.WriteLine($"Энергия заполнена: {(StarRailStaminaManager.IsStaminaFull(data) ? "Да" : "Нет")}");

            // Время восстановления
            var recoveryTime = StarRailStaminaManager.GetStaminaRecoveryTime(data);
            var fullTime = StarRailStaminaManager.GetStaminaFullTime(data);
            Console.WriteLine($"Время до полного восстановления: {recoveryTime.Hours:D2}:{recoveryTime.Minutes:D2}:{recoveryTime.Seconds:D2}");
            Console.WriteLine($"Энергия будет полной в: {fullTime:HH:mm:ss} ({fullTime:dd.MM.yyyy})");

            // Следующая единица энергии
            var nextPointTime = StarRailStaminaManager.GetNextStaminaPointTime(data);
            var timeToNextPoint = StarRailStaminaManager.GetTimeToNextStaminaPoint(data);
            Console.WriteLine($"Следующая единица энергии в: {nextPointTime:HH:mm:ss}");
            Console.WriteLine($"Время до следующей единицы: {timeToNextPoint.Minutes:D2}:{timeToNextPoint.Seconds:D2}");

            // Прогнозы
            var staminaIn1Hour = StarRailStaminaManager.GetStaminaAtTime(data, TimeSpan.FromHours(1));
            var staminaIn3Hours = StarRailStaminaManager.GetStaminaAtTime(data, TimeSpan.FromHours(3));
            var staminaIn6Hours = StarRailStaminaManager.GetStaminaAtTime(data, TimeSpan.FromHours(6));

            Console.WriteLine($"\n=== ПРОГНОЗЫ ===");
            Console.WriteLine($"Энергия через 1 час: {staminaIn1Hour}");
            Console.WriteLine($"Энергия через 3 часа: {staminaIn3Hours}");
            Console.WriteLine($"Энергия через 6 часов: {staminaIn6Hours}");

            // Рекомендации
            Console.WriteLine($"\n=== РЕКОМЕНДАЦИИ ===");
            Console.WriteLine("• Используйте энергию эффективно");

            // Дополнительная информация
            Console.WriteLine($"\n=== ДОПОЛНИТЕЛЬНАЯ ИНФОРМАЦИЯ ===");
            Console.WriteLine($"Экспедиции: {data.AcceptedExpeditionNum}/{data.TotalExpeditionNum}");
            Console.WriteLine($"Очки тренировки: {data.CurrentTrainScore}/{data.MaxTrainScore}");
            Console.WriteLine($"Очки подземелий: {data.CurrentRogueScore}/{data.MaxRogueScore}");

            if (data.Expeditions != null && data.Expeditions.Length > 0)
            {
                Console.WriteLine($"\n=== ЭКСПЕДИЦИИ ===");
                foreach (var expedition in data.Expeditions)
                {
                    var remainingTime = StarRailClient.GetExpeditionRemainingTime(expedition);
                    var completionTime = StarRailClient.GetExpeditionCompletionTime(expedition);
                    Console.WriteLine($"• {expedition.Name}: {expedition.Status}");
                    Console.WriteLine($"  Осталось времени: {remainingTime}");
                    Console.WriteLine($"  Завершится в: {completionTime:HH:mm:ss}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении данных: {ex.Message}");
            Console.WriteLine($"Тип ошибки: {ex.GetType().Name}");
        }
    }
}