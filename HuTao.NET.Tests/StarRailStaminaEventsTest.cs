using HuTao.NET.GI.Models.HonkaiStarRail;
using HuTao.NET.GI;

namespace HuTao.NET.Tests;

/// <summary>
/// Тесты для событий энергии в Honkai Star Rail
/// </summary>
public class StarRailStaminaEventsTest
{
    [Fact]
    public void TestStaminaManager()
    {
        // Создаем тестовые данные
        var testData = new StarRailDailyNoteData
        {
            CurrentStamina = 150,
            MaxStamina = 300,
            StaminaRecoverTime = 1800, // 30 минут
            StaminaFullTimestamp = DateTimeOffset.Now.AddHours(2).ToUnixTimeSeconds(),
            CurrentTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
        };

        // Тестируем основные методы
        Console.WriteLine("=== Тестирование менеджера энергии Star Rail ===");
        Console.WriteLine($"Текущая энергия: {testData.CurrentStamina}/{testData.MaxStamina}");
        Console.WriteLine($"Процент заполнения: {StarRailStaminaManager.GetStaminaPercentage(testData):P1}");
        Console.WriteLine($"Энергия заполнена: {StarRailStaminaManager.IsStaminaFull(testData)}");

        // Время восстановления
        var recoveryTime = StarRailStaminaManager.GetStaminaRecoveryTime(testData);
        Console.WriteLine($"Время до полного восстановления: {recoveryTime.Hours:D2}:{recoveryTime.Minutes:D2}:{recoveryTime.Seconds:D2}");
        Console.WriteLine($"Форматированное время: {recoveryTime.Hours:D2}:{recoveryTime.Minutes:D2}:{recoveryTime.Seconds:D2}");

        // Время полного восстановления
        var fullTime = StarRailStaminaManager.GetStaminaFullTime(testData);
        Console.WriteLine($"Время полного восстановления: {fullTime:HH:mm:ss}");

        // Следующая единица энергии
        var nextPointTime = StarRailStaminaManager.GetNextStaminaPointTime(testData);
        var timeToNextPoint = StarRailStaminaManager.GetTimeToNextStaminaPoint(testData);
        Console.WriteLine($"Следующая единица энергии в: {nextPointTime:HH:mm:ss}");
        Console.WriteLine($"Время до следующей единицы: {timeToNextPoint.Minutes:D2}:{timeToNextPoint.Seconds:D2}");
        Console.WriteLine($"Форматированное время: {timeToNextPoint.Minutes:D2}:{timeToNextPoint.Seconds:D2}");

        // Расчет энергии через время
        var staminaIn1Hour = StarRailStaminaManager.GetStaminaAtTime(testData, TimeSpan.FromHours(1));
        var staminaIn2Hours = StarRailStaminaManager.GetStaminaAtTime(testData, TimeSpan.FromHours(2));
        Console.WriteLine($"Энергия через 1 час: {staminaIn1Hour}");
        Console.WriteLine($"Энергия через 2 часа: {staminaIn2Hours}");

        // Время достижения целевой энергии
        var timeTo200 = StarRailStaminaManager.GetTimeToReachStamina(testData, 200);
        var timeSpanTo200 = StarRailStaminaManager.GetTimeSpanToReachStamina(testData, 200);
        Console.WriteLine($"Время достижения 200 энергии: {timeTo200:HH:mm:ss}");
        Console.WriteLine($"Время до 200 энергии: {timeSpanTo200.Hours:D2}:{timeSpanTo200.Minutes:D2}:{timeSpanTo200.Seconds:D2}");

        var timeToFull = StarRailStaminaManager.GetTimeToReachStamina(testData, 300);
        var timeSpanToFull = StarRailStaminaManager.GetTimeSpanToReachStamina(testData, 300);
        Console.WriteLine($"Время достижения полной энергии: {timeToFull:HH:mm:ss}");
        Console.WriteLine($"Время до полной энергии: {timeSpanToFull.Hours:D2}:{timeSpanToFull.Minutes:D2}:{timeSpanToFull.Seconds:D2}");

        // Тестируем новые методы
        var staminaInfo = $"Энергия: {testData.CurrentStamina}/{testData.MaxStamina} ({StarRailStaminaManager.GetStaminaPercentage(testData):P1})";
        Console.WriteLine($"Информация о энергии: {staminaInfo}");

        Console.WriteLine("Рекомендации:");
        Console.WriteLine("  Используйте энергию эффективно");

        var forecast = StarRailStaminaManager.GetStaminaAtTime(testData, TimeSpan.FromHours(3));
        Console.WriteLine($"Прогноз энергии через 3 часа: {forecast}");

        Console.WriteLine("=== Тест завершен ===");
    }

    [Fact]
    public void TestStaminaCalculationAccuracy()
    {

        // Тестируем точность расчетов
        var testData = new StarRailDailyNoteData
        {
            CurrentStamina = 0,
            MaxStamina = 300,
            StaminaRecoverTime = 0,
            CurrentTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
        };

        // Проверяем, что через 6 минут получаем 1 энергию
        var staminaAfter6Minutes = StarRailStaminaManager.GetStaminaAtTime(testData, TimeSpan.FromMinutes(6));
        Assert.Equal(1, staminaAfter6Minutes);

        // Проверяем, что через 1 час получаем 10 энергии (60/6 = 10)
        var staminaAfter1Hour = StarRailStaminaManager.GetStaminaAtTime(testData, TimeSpan.FromHours(1));
        Assert.Equal(10, staminaAfter1Hour);

        // Проверяем, что через 30 часов получаем максимум энергии
        var staminaAfter30Hours = StarRailStaminaManager.GetStaminaAtTime(testData, TimeSpan.FromHours(30));
        Assert.Equal(300, staminaAfter30Hours);
    }

    [Fact]
    public void TestEdgeCases()
    {

        var testData = new StarRailDailyNoteData
        {
            CurrentStamina = 300,
            MaxStamina = 300,
            StaminaRecoverTime = 0,
            CurrentTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
        };

        // Тестируем граничные случаи
        Assert.True(StarRailStaminaManager.IsStaminaFull(testData));
        Assert.Equal(1.0, StarRailStaminaManager.GetStaminaPercentage(testData));

        // Если энергия уже заполнена, время до следующей единицы должно быть 0
        var timeToNext = StarRailStaminaManager.GetTimeToNextStaminaPoint(testData);
        Assert.Equal(TimeSpan.Zero, timeToNext);

        // Если запрашиваем энергию меньше текущей, должно вернуть текущее время
        var timeToReach = StarRailStaminaManager.GetTimeToReachStamina(testData, 200);
        var currentTime = DateTimeOffset.FromUnixTimeSeconds(testData.CurrentTimestamp).DateTime;
        Assert.Equal(currentTime, timeToReach);
    }

    [Fact]
    public void TestStaminaEventsSubscription()
    {
        Console.WriteLine("=== Тестирование подписки на события энергии ===");

        // Создаем тестовый клиент через фабричный метод
        var cookie = new Cookie { LToken = "test", LtUid = "test" };
        using var client = HuTaoClient.Create(cookie);
        var starRailClient = client.StarRail;

        // Создаем тестовые данные
        var testData = new StarRailDailyNoteData
        {
            CurrentStamina = 299,
            MaxStamina = 300,
            StaminaRecoverTime = 360, // 6 минут
            StaminaFullTimestamp = DateTimeOffset.Now.AddMinutes(6).ToUnixTimeSeconds(),
            CurrentTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
        };

        // Флаги для отслеживания вызовов событий
        bool staminaFullCalled = false;
        bool staminaPointReceivedCalled = false;
        bool staminaChangedCalled = false;

        // Подписываемся на события
        starRailClient.StaminaFull += (sender, args) =>
        {
            Console.WriteLine($"Событие: Энергия заполнена!");
            Console.WriteLine($"  Время: {args.FullTime:HH:mm:ss}");
            Console.WriteLine($"  Максимум: {args.MaxStamina}");
            Console.WriteLine($"  Время восстановления: {args.RecoveryTime}");
            staminaFullCalled = true;
        };

        starRailClient.StaminaPointReceived += (sender, args) =>
        {
            Console.WriteLine($"Событие: Получена единица энергии!");
            Console.WriteLine($"  Время: {args.PointTime:HH:mm:ss}");
            Console.WriteLine($"  Новое значение: {args.NewStaminaValue}");
            Console.WriteLine($"  Процент: {args.StaminaPercentage:P1}");
            Console.WriteLine($"  Время до следующей: {args.TimeToNextPoint}");
            staminaPointReceivedCalled = true;
        };

        starRailClient.StaminaChanged += (sender, args) =>
        {
            Console.WriteLine($"Событие: Изменение энергии!");
            Console.WriteLine($"  Предыдущее: {args.PreviousStamina}");
            Console.WriteLine($"  Текущее: {args.CurrentStamina}");
            Console.WriteLine($"  Изменение: {args.StaminaChange}");
            Console.WriteLine($"  Процент: {args.StaminaPercentage:P1}");
            Console.WriteLine($"  Время: {args.ChangeTime:HH:mm:ss}");
            staminaChangedCalled = true;
        };

        // Проверяем события с предыдущим значением 298 (получение единицы энергии)
        starRailClient.CheckAndTriggerStaminaEvents(testData, 298);

        // Проверяем, что события были вызваны
        Assert.True(staminaPointReceivedCalled, "Событие получения единицы энергии должно быть вызвано");
        Assert.True(staminaChangedCalled, "Событие изменения энергии должно быть вызвано");

        // Тестируем достижение максимальной энергии
        var fullStaminaData = new StarRailDailyNoteData
        {
            CurrentStamina = 300,
            MaxStamina = 300,
            StaminaRecoverTime = 0,
            StaminaFullTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
            CurrentTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
        };

        // Сбрасываем флаги
        staminaFullCalled = false;
        staminaPointReceivedCalled = false;
        staminaChangedCalled = false;

        // Проверяем события с полной энергией
        starRailClient.CheckAndTriggerStaminaEvents(fullStaminaData, 299);

        // Проверяем, что событие максимальной энергии было вызвано
        Assert.True(staminaFullCalled, "Событие максимальной энергии должно быть вызвано");
        Assert.True(staminaChangedCalled, "Событие изменения энергии должно быть вызвано");

        Console.WriteLine("=== Тест событий завершен ===");
    }

    [Fact]
    public void TestStaminaEventArguments()
    {
        Console.WriteLine("=== Тестирование аргументов событий ===");

        // Создаем тестовые данные
        var testData = new StarRailDailyNoteData
        {
            CurrentStamina = 150,
            MaxStamina = 300,
            StaminaRecoverTime = 1800,
            StaminaFullTimestamp = DateTimeOffset.Now.AddHours(2).ToUnixTimeSeconds(),
            CurrentTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
        };

        // Тестируем аргументы события изменения энергии
        var changeArgs = new StaminaChangedEventArgs(140, 150, 300, DateTime.Now);

        Assert.Equal(140, changeArgs.PreviousStamina);
        Assert.Equal(150, changeArgs.CurrentStamina);
        Assert.Equal(300, changeArgs.MaxStamina);
        Assert.Equal(10, changeArgs.StaminaChange);
        Assert.Equal(0.5, changeArgs.StaminaPercentage);

        Console.WriteLine($"Изменение энергии: {changeArgs.StaminaChange}");
        Console.WriteLine($"Процент заполнения: {changeArgs.StaminaPercentage:P1}");

        // Тестируем аргументы события получения единицы энергии
        var pointArgs = new StaminaPointReceivedEventArgs(
            DateTime.Now.AddMinutes(6),
            151,
            300,
            TimeSpan.FromMinutes(6)
        );

        Assert.Equal(151, pointArgs.NewStaminaValue);
        Assert.Equal(300, pointArgs.MaxStamina);
        Assert.Equal(TimeSpan.FromMinutes(6), pointArgs.TimeToNextPoint);
        Assert.Equal(151.0 / 300.0, pointArgs.StaminaPercentage);

        Console.WriteLine($"Новое значение энергии: {pointArgs.NewStaminaValue}");
        Console.WriteLine($"Время до следующей единицы: {pointArgs.TimeToNextPoint}");

        // Тестируем аргументы события максимальной энергии
        var fullArgs = new StaminaFullEventArgs(
            DateTime.Now.AddHours(2),
            300,
            TimeSpan.FromHours(2)
        );

        Assert.Equal(300, fullArgs.MaxStamina);
        Assert.Equal(TimeSpan.FromHours(2), fullArgs.RecoveryTime);

        Console.WriteLine($"Максимальная энергия: {fullArgs.MaxStamina}");
        Console.WriteLine($"Время восстановления: {fullArgs.RecoveryTime}");

        Console.WriteLine("=== Тест аргументов завершен ===");
    }
}