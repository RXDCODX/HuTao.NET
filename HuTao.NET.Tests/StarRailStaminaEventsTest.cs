using HuTao.NET.Models.HonkaiStarRail;
using Xunit.Abstractions;

namespace HuTao.NET.Tests;

/// <summary>
/// Tests for stamina events in Honkai Star Rail
/// </summary>
public class StarRailStaminaEventsTest(ITestOutputHelper testOutputHelper)
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
            CurrentTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
        };

        // Test main methods
        testOutputHelper.WriteLine("=== Testing Star Rail Stamina Manager ===");
        testOutputHelper.WriteLine($"Current Stamina: {testData.CurrentStamina}/{testData.MaxStamina}");
        testOutputHelper.WriteLine(
            $"Stamina Percentage: {StarRailStaminaManager.GetStaminaPercentage(testData):P1}"
        );
        testOutputHelper.WriteLine($"Is Stamina Full: {StarRailStaminaManager.IsStaminaFull(testData)}");

        // Recovery time
        var recoveryTime = StarRailStaminaManager.GetStaminaRecoveryTime(testData);
        testOutputHelper.WriteLine(
            $"Time to Full Recovery: {recoveryTime.Hours:D2}:{recoveryTime.Minutes:D2}:{recoveryTime.Seconds:D2}"
        );
        testOutputHelper.WriteLine(
            $"Formatted Time: {recoveryTime.Hours:D2}:{recoveryTime.Minutes:D2}:{recoveryTime.Seconds:D2}"
        );

        // Full recovery time
        var fullTime = StarRailStaminaManager.GetStaminaFullTime(testData);
        testOutputHelper.WriteLine($"Full Recovery Time: {fullTime:HH:mm:ss}");

        // Next stamina point
        var nextPointTime = StarRailStaminaManager.GetNextStaminaPointTime(testData);
        var timeToNextPoint = StarRailStaminaManager.GetTimeToNextStaminaPoint(testData);
        testOutputHelper.WriteLine($"Next Stamina Point at: {nextPointTime:HH:mm:ss}");
        testOutputHelper.WriteLine(
            $"Time to Next Point: {timeToNextPoint.Minutes:D2}:{timeToNextPoint.Seconds:D2}"
        );
        testOutputHelper.WriteLine(
            $"Formatted Time: {timeToNextPoint.Minutes:D2}:{timeToNextPoint.Seconds:D2}"
        );

        // Stamina calculation over time
        var staminaIn1Hour = StarRailStaminaManager.GetStaminaAtTime(
            testData,
            TimeSpan.FromHours(1)
        );
        var staminaIn2Hours = StarRailStaminaManager.GetStaminaAtTime(
            testData,
            TimeSpan.FromHours(2)
        );
        testOutputHelper.WriteLine($"Stamina in 1 hour: {staminaIn1Hour}");
        testOutputHelper.WriteLine($"Stamina in 2 hours: {staminaIn2Hours}");

        // Time to reach target stamina
        var timeTo200 = StarRailStaminaManager.GetTimeToReachStamina(testData, 200);
        var timeSpanTo200 = StarRailStaminaManager.GetTimeSpanToReachStamina(testData, 200);
        testOutputHelper.WriteLine($"Time to reach 200 stamina: {timeTo200:HH:mm:ss}");
        testOutputHelper.WriteLine(
            $"Time to 200 stamina: {timeSpanTo200.Hours:D2}:{timeSpanTo200.Minutes:D2}:{timeSpanTo200.Seconds:D2}"
        );

        var timeToFull = StarRailStaminaManager.GetTimeToReachStamina(testData, 300);
        var timeSpanToFull = StarRailStaminaManager.GetTimeSpanToReachStamina(testData, 300);
        testOutputHelper.WriteLine($"Time to reach full stamina: {timeToFull:HH:mm:ss}");
        testOutputHelper.WriteLine(
            $"Time to full stamina: {timeSpanToFull.Hours:D2}:{timeSpanToFull.Minutes:D2}:{timeSpanToFull.Seconds:D2}"
        );

        // Test new methods
        var staminaInfo =
            $"Stamina: {testData.CurrentStamina}/{testData.MaxStamina} ({StarRailStaminaManager.GetStaminaPercentage(testData):P1})";
        testOutputHelper.WriteLine($"Stamina Info: {staminaInfo}");

        testOutputHelper.WriteLine("Recommendations:");
        testOutputHelper.WriteLine("  Use stamina efficiently");

        var forecast = StarRailStaminaManager.GetStaminaAtTime(testData, TimeSpan.FromHours(3));
        testOutputHelper.WriteLine($"Stamina forecast in 3 hours: {forecast}");

        testOutputHelper.WriteLine("=== Test completed ===");
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
            CurrentTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
        };

        // Check that we get 1 stamina after 6 minutes
        var staminaAfter6Minutes = StarRailStaminaManager.GetStaminaAtTime(
            testData,
            TimeSpan.FromMinutes(6)
        );
        Assert.Equal(1, staminaAfter6Minutes);

        // Check that we get 10 stamina after 1 hour (60/6 = 10)
        var staminaAfter1Hour = StarRailStaminaManager.GetStaminaAtTime(
            testData,
            TimeSpan.FromHours(1)
        );
        Assert.Equal(10, staminaAfter1Hour);

        // Check that we get maximum stamina after 30 hours
        var staminaAfter30Hours = StarRailStaminaManager.GetStaminaAtTime(
            testData,
            TimeSpan.FromHours(30)
        );
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
            CurrentTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
        };

        // Test edge cases
        Assert.True(StarRailStaminaManager.IsStaminaFull(testData));
        Assert.Equal(1.0, StarRailStaminaManager.GetStaminaPercentage(testData));

        // If stamina is already full, time to next point should be 0
        var timeToNext = StarRailStaminaManager.GetTimeToNextStaminaPoint(testData);
        Assert.Equal(TimeSpan.Zero, timeToNext);

        // If we request stamina less than current, should return current time
        var timeToReach = StarRailStaminaManager.GetTimeToReachStamina(testData, 200);
        var currentTime = DateTimeOffset.FromUnixTimeSeconds(testData.CurrentTimestamp).DateTime;
        Assert.Equal(currentTime, timeToReach);
    }

    [Fact]
    public void TestStaminaEventsSubscription()
    {
        testOutputHelper.WriteLine("=== Testing Stamina Events Subscription ===");

        // Create test client through factory method
        var cookie = new Cookie { LToken = "test", LtUid = "test" };
        using var client = HuTaoClient.Create(cookie);
        var starRailClient = client.StarRail;

        // Create test data
        var testData = new StarRailDailyNoteData
        {
            CurrentStamina = 299,
            MaxStamina = 300,
            StaminaRecoverTime = 360, // 6 minutes
            StaminaFullTimestamp = DateTimeOffset.Now.AddMinutes(6).ToUnixTimeSeconds(),
            CurrentTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
        };

        // Flags for tracking event calls
        var staminaFullCalled = false;
        var staminaPointReceivedCalled = false;
        var staminaChangedCalled = false;

        // Subscribe to events
        starRailClient.StaminaFull += (sender, args) =>
        {
            testOutputHelper.WriteLine($"Event: Stamina Full!");
            testOutputHelper.WriteLine($"  Time: {args.FullTime:HH:mm:ss}");
            testOutputHelper.WriteLine($"  Maximum: {args.MaxStamina}");
            testOutputHelper.WriteLine($"  Recovery Time: {args.RecoveryTime}");
            staminaFullCalled = true;
        };

        starRailClient.StaminaPointReceived += (sender, args) =>
        {
            testOutputHelper.WriteLine($"Event: Stamina Point Received!");
            testOutputHelper.WriteLine($"  Time: {args.PointTime:HH:mm:ss}");
            testOutputHelper.WriteLine($"  New Value: {args.NewStaminaValue}");
            testOutputHelper.WriteLine($"  Percentage: {args.StaminaPercentage:P1}");
            testOutputHelper.WriteLine($"  Time to Next: {args.TimeToNextPoint}");
            staminaPointReceivedCalled = true;
        };

        starRailClient.StaminaChanged += (sender, args) =>
        {
            testOutputHelper.WriteLine($"Event: Stamina Changed!");
            testOutputHelper.WriteLine($"  Previous: {args.PreviousStamina}");
            testOutputHelper.WriteLine($"  Current: {args.CurrentStamina}");
            testOutputHelper.WriteLine($"  Change: {args.StaminaChange}");
            testOutputHelper.WriteLine($"  Percentage: {args.StaminaPercentage:P1}");
            testOutputHelper.WriteLine($"  Time: {args.ChangeTime:HH:mm:ss}");
            staminaChangedCalled = true;
        };

        // Check events with previous value 298 (receiving stamina point)
        starRailClient.CheckAndTriggerStaminaEvents(testData, 298);

        // Check that events were called
        Assert.True(
            staminaPointReceivedCalled,
            "Stamina point received event should be called"
        );
        Assert.True(staminaChangedCalled, "Stamina changed event should be called");

        // Test reaching maximum stamina
        var fullStaminaData = new StarRailDailyNoteData
        {
            CurrentStamina = 300,
            MaxStamina = 300,
            StaminaRecoverTime = 0,
            StaminaFullTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
            CurrentTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
        };

        // Reset flags
        staminaFullCalled = false;
        staminaPointReceivedCalled = false;
        staminaChangedCalled = false;

        // Check events with full stamina
        starRailClient.CheckAndTriggerStaminaEvents(fullStaminaData, 299);

        // Check that maximum stamina event was called
        Assert.True(staminaFullCalled, "Maximum stamina event should be called");
        Assert.True(staminaChangedCalled, "Stamina changed event should be called");

        testOutputHelper.WriteLine("=== Events test completed ===");
    }

    [Fact]
    public void TestStaminaEventArguments()
    {
        testOutputHelper.WriteLine("=== Testing Event Arguments ===");

        // Create test data
        var testData = new StarRailDailyNoteData
        {
            CurrentStamina = 150,
            MaxStamina = 300,
            StaminaRecoverTime = 1800,
            StaminaFullTimestamp = DateTimeOffset.Now.AddHours(2).ToUnixTimeSeconds(),
            CurrentTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
        };

        // Test stamina changed event arguments
        var changeArgs = new StaminaChangedEventArgs(140, 150, 300, DateTime.Now);

        Assert.Equal(140, changeArgs.PreviousStamina);
        Assert.Equal(150, changeArgs.CurrentStamina);
        Assert.Equal(300, changeArgs.MaxStamina);
        Assert.Equal(10, changeArgs.StaminaChange);
        Assert.Equal(0.5, changeArgs.StaminaPercentage);

        testOutputHelper.WriteLine($"Stamina Change: {changeArgs.StaminaChange}");
        testOutputHelper.WriteLine($"Stamina Percentage: {changeArgs.StaminaPercentage:P1}");

        // Test stamina point received event arguments
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

        testOutputHelper.WriteLine($"New Stamina Value: {pointArgs.NewStaminaValue}");
        testOutputHelper.WriteLine($"Time to Next Point: {pointArgs.TimeToNextPoint}");

        // Test maximum stamina event arguments
        var fullArgs = new StaminaFullEventArgs(
            DateTime.Now.AddHours(2),
            300,
            TimeSpan.FromHours(2)
        );

        Assert.Equal(300, fullArgs.MaxStamina);
        Assert.Equal(TimeSpan.FromHours(2), fullArgs.RecoveryTime);

        testOutputHelper.WriteLine($"Maximum Stamina: {fullArgs.MaxStamina}");
        testOutputHelper.WriteLine($"Recovery Time: {fullArgs.RecoveryTime}");

        testOutputHelper.WriteLine("=== Event arguments test completed ===");
    }
}
