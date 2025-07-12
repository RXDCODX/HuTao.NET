# HuTao.NET

A .NET wrapper library for HoyoLab's WebAPI to retrieve data about Genshin Impact, Honkai Star Rail, and Zenless Zone Zero.

## Features

- **Multi-Game Support**: Support for Genshin Impact, Honkai Star Rail, and Zenless Zone Zero
- **User Statistics**: Get detailed game statistics and character information
- **Daily Notes**: Retrieve daily resin/energy status and expedition information
- **Stamina Management**: Track stamina recovery time and full time for Star Rail
- **Expedition Tracking**: Monitor expedition progress and completion times
- **Daily Rewards**: Claim daily login rewards automatically
- **Account Management**: Get user account information and game roles
- **Server Detection**: Automatic server detection based on UID

## Installation

```bash
dotnet add package HuTao.NET
```

## Quick Start

### Using Root Client (Recommended)

```csharp
using HuTao.NET.GI;

// Create cookie with your HoyoLab credentials
var cookie = new Cookie
{
    ltoken = "your_ltoken_here",
    ltuid = "your_ltuid_here"
};

// Create root client that manages all game clients
using var client = HuTaoClient.Create(cookie);

// Set language for all requests
client.SetLanguage("en-us");

// Get user stats for all games
var userStats = await client.FetchUserStats();

// Get account information
var accountInfo = await client.GetUserAccountInfoByLToken();

// Get game roles
var gameRoles = await client.GetGameRoles();

// Access individual game clients
var genshinClient = client.Genshin;
var starRailClient = client.StarRail;
var zenlessClient = client.Zenless;
```

### Genshin Impact

```csharp
using HuTao.NET.GI;

// Create cookie with your HoyoLab credentials
var cookie = new Cookie
{
    ltoken = "your_ltoken_here",
    ltuid = "your_ltuid_here"
};

// Create Genshin client (using root client is recommended)
var genshinClient = new GenshinClient(cookie);

// Get user stats
var userStats = await genshinClient.FetchUserStats();

// Create Genshin user from UID
var genshinUser = new GenshinUser(123456789);

// Get game statistics
var stats = await genshinClient.FetchGenshinStats(genshinUser);

// Get daily note (resin, expeditions, etc.)
var dailyNote = await genshinClient.FetchDailyNote(genshinUser);

// Claim daily reward
var reward = await genshinClient.ClaimDailyReward(genshinUser);
```

### Honkai Star Rail

```csharp
using HuTao.NET.GI;

// Using root client
using var client = HuTaoClient.Create(cookie);
var starRailClient = client.StarRail;

// Create Star Rail user from UID
var starRailUser = new StarRailUser(123456789);

// Get game statistics
var stats = await starRailClient.FetchStarRailStats(starRailUser);

// Get daily note (stamina, expeditions, etc.)
var dailyNote = await starRailClient.FetchDailyNote(starRailUser);

// Check stamina status
var staminaPercentage = StarRailClient.GetStaminaPercentage(dailyNote.data);
var isStaminaFull = StarRailClient.IsStaminaFull(dailyNote.data);
var staminaRecoveryTime = StarRailClient.GetStaminaRecoveryTime(dailyNote.data);
var staminaFullTime = StarRailClient.GetStaminaFullTime(dailyNote.data);

Console.WriteLine($"Stamina: {dailyNote.data.CurrentStamina}/{dailyNote.data.MaxStamina} ({staminaPercentage:F1}%)");
Console.WriteLine($"Stamina full: {isStaminaFull}");
Console.WriteLine($"Recovery time: {staminaRecoveryTime}");
Console.WriteLine($"Full at: {staminaFullTime}");

// Advanced stamina tracking (NEW!)
var nextStaminaPoint = StarRailClient.GetNextStaminaPointTime(dailyNote.data);
var timeToNextPoint = StarRailClient.GetTimeToNextStaminaPoint(dailyNote.data);
var staminaIn1Hour = StarRailClient.GetStaminaAtTime(dailyNote.data, TimeSpan.FromHours(1));
var timeTo200Stamina = StarRailClient.GetTimeToReachStamina(dailyNote.data, 200);

Console.WriteLine($"Next stamina point: {nextStaminaPoint:HH:mm:ss}");
Console.WriteLine($"Time to next point: {timeToNextPoint.Minutes:D2}:{timeToNextPoint.Seconds:D2}");
Console.WriteLine($"Stamina in 1 hour: {staminaIn1Hour}");
Console.WriteLine($"Time to reach 200 stamina: {timeTo200Stamina:HH:mm:ss}");

// Check expeditions
if (dailyNote.data.Expeditions != null)
{
    foreach (var expedition in dailyNote.data.Expeditions)
    {
        var remainingTime = StarRailClient.GetExpeditionRemainingTime(expedition);
        var completionTime = StarRailClient.GetExpeditionCompletionTime(expedition);
        Console.WriteLine($"Expedition: {expedition.Name} - {expedition.Status}");
        Console.WriteLine($"Remaining time: {remainingTime}");
        Console.WriteLine($"Completion time: {completionTime}");
    }
}

// Get reward data (new API)
var rewardData = await starRailClient.GetStarRailRewardData();

// Claim daily reward
var reward = await starRailClient.ClaimDailyReward(starRailUser);
```

### Star Rail Stamina Events (NEW!)

Honkai Star Rail features advanced stamina tracking capabilities:

```csharp
// Get daily note
var dailyNote = await starRailClient.FetchDailyNote(starRailUser);

// Basic stamina information
Console.WriteLine($"Current Stamina: {dailyNote.data.CurrentStamina}/{dailyNote.data.MaxStamina}");
Console.WriteLine($"Stamina Full: {StarRailClient.IsStaminaFull(dailyNote.data)}");
Console.WriteLine($"Stamina Percentage: {StarRailClient.GetStaminaPercentage(dailyNote.data):P1}");

// Time calculations
var recoveryTime = StarRailClient.GetStaminaRecoveryTime(dailyNote.data);
var fullTime = StarRailClient.GetStaminaFullTime(dailyNote.data);
Console.WriteLine($"Time to full recovery: {recoveryTime.Hours:D2}:{recoveryTime.Minutes:D2}:{recoveryTime.Seconds:D2}");
Console.WriteLine($"Stamina will be full at: {fullTime:HH:mm:ss}");

// Next stamina point (energy recovers every 6 minutes)
var nextPointTime = StarRailClient.GetNextStaminaPointTime(dailyNote.data);
var timeToNextPoint = StarRailClient.GetTimeToNextStaminaPoint(dailyNote.data);
Console.WriteLine($"Next stamina point at: {nextPointTime:HH:mm:ss}");
Console.WriteLine($"Time to next point: {timeToNextPoint.Minutes:D2}:{timeToNextPoint.Seconds:D2}");

// Future stamina calculations
var staminaIn30Minutes = StarRailClient.GetStaminaAtTime(dailyNote.data, TimeSpan.FromMinutes(30));
var staminaIn2Hours = StarRailClient.GetStaminaAtTime(dailyNote.data, TimeSpan.FromHours(2));
Console.WriteLine($"Stamina in 30 minutes: {staminaIn30Minutes}");
Console.WriteLine($"Stamina in 2 hours: {staminaIn2Hours}");

// Time to reach specific stamina values
var timeTo150 = StarRailClient.GetTimeToReachStamina(dailyNote.data, 150);
var timeTo250 = StarRailClient.GetTimeToReachStamina(dailyNote.data, 250);
Console.WriteLine($"Time to reach 150 stamina: {timeTo150:HH:mm:ss}");
Console.WriteLine($"Time to reach 250 stamina: {timeTo250:HH:mm:ss}");

// Formatted time strings
var recoveryTimeString = StarRailClient.GetStaminaRecoveryTimeString(dailyNote.data);
var nextPointTimeString = StarRailClient.GetNextStaminaPointTimeString(dailyNote.data);
Console.WriteLine($"Formatted recovery time: {recoveryTimeString}");
Console.WriteLine($"Formatted next point time: {nextPointTimeString}");
```

**Available Stamina Event Methods:**

- `GetNextStaminaPointTime()` - Get exact time when next stamina point will be received
- `GetTimeToNextStaminaPoint()` - Get time remaining until next stamina point
- `GetStaminaAtTime(TimeSpan)` - Calculate stamina at a specific time in the future
- `GetTimeToReachStamina(int)` - Get time when stamina will reach a specific value
- `GetTimeSpanToReachStamina(int)` - Get time span until stamina reaches a specific value
- `GetStaminaRecoveryTimeString()` - Get formatted string of recovery time
- `GetNextStaminaPointTimeString()` - Get formatted string of time to next point

**Stamina Events (NEW!):**

```csharp
// Create Star Rail client
using var client = HuTaoClient.Create(cookie);
var starRailClient = client.StarRail;

// Subscribe to stamina events
starRailClient.StaminaFull += (sender, args) =>
{
    Console.WriteLine($"Stamina is full at {args.FullTime:HH:mm:ss}!");
    Console.WriteLine($"Max stamina: {args.MaxStamina}");
    Console.WriteLine($"Recovery time: {args.RecoveryTime}");
};

starRailClient.StaminaPointReceived += (sender, args) =>
{
    Console.WriteLine($"Stamina point received at {args.PointTime:HH:mm:ss}!");
    Console.WriteLine($"New stamina value: {args.NewStaminaValue}");
    Console.WriteLine($"Stamina percentage: {args.StaminaPercentage:P1}");
    Console.WriteLine($"Time to next point: {args.TimeToNextPoint}");
};

starRailClient.StaminaChanged += (sender, args) =>
{
    Console.WriteLine($"Stamina changed from {args.PreviousStamina} to {args.CurrentStamina}");
    Console.WriteLine($"Change: {args.StaminaChange}");
    Console.WriteLine($"Percentage: {args.StaminaPercentage:P1}");
};

// Get daily note and check events
var dailyNote = await starRailClient.FetchDailyNote(starRailUser);
starRailClient.CheckAndTriggerStaminaEvents(dailyNote.data, previousStamina);
```

**Available Events:**
- `StaminaFull` - Triggered when stamina reaches maximum
- `StaminaPointReceived` - Triggered when a single stamina point is received
- `StaminaChanged` - Triggered when stamina value changes

**Stamina Recovery Rate:**
- Energy recovers every 6 minutes (360 seconds)
- Maximum stamina is 300
- Full recovery takes 30 hours (300 × 6 minutes)
```

### Zenless Zone Zero (Future)

```csharp
using HuTao.NET.GI;

// Using root client
using var client = HuTaoClient.Create(cookie);
var zenlessClient = client.Zenless;

// Create Zenless user from UID
var zenlessUser = new ZenlessUser(123456789);

// Get game statistics (when available)
var stats = await zenlessClient.FetchZenlessStats(zenlessUser);

// Get daily note (when available)
var dailyNote = await zenlessClient.FetchDailyNote(zenlessUser);
```

## Cookie Management

### Method 1: Using Cookie class
```csharp
var cookie = new Cookie
{
    ltoken = "your_ltoken_here",
    ltuid = "your_ltuid_here"
};
```

### Method 2: Using CookieV2 class (for newer tokens)
```csharp
var cookie = new CookieV2
{
    ltoken_v2 = "your_ltoken_v2_here",
    ltmid_v2 = "your_ltmid_v2_here",
    ltuid_v2 = "your_ltuid_v2_here"
};
```

### Method 3: Using raw cookie string
```csharp
var cookie = new RawCookie("your_cookie_string_here", "your_hoyolab_uid_here");
```

## Error Handling

The library provides specific exception types for different error scenarios:

```csharp
try
{
    var stats = await client.FetchGenshinStats(user);
}
catch (AccountNotFoundException ex)
{
    // Account not found or invalid UID
}
catch (HoyoLabAPIBadRequestException ex)
{
    // API returned an error
    Console.WriteLine($"Error: {ex.Message}, Code: {ex.Retcode}");
}
catch (HoyoLabCaptchaBlockException ex)
{
    // Account is blocked by captcha
}
catch (DailyRewardAlreadyReceivedException ex)
{
    // Daily reward already claimed
}
```

## Language Support

You can set the language for API responses:

```csharp
var clientData = new ClientData
{
    Language = "en-us" // or "zh-cn", "ja-jp", etc.
};

var client = new GenshinClient(cookie, clientData);
```

## Available Languages

Get available languages:

```csharp
var languages = await HuTaoClient.GetAvailableLanguages();
```

## Advanced Usage

### Working with Multiple Games

```csharp
using var client = HuTaoClient.Create(cookie);

// Get all user stats
var userStats = await client.FetchUserStats();

// Work with Genshin Impact
var genshinUser = new GenshinUser(123456789);
var genshinStats = await client.Genshin.FetchGenshinStats(genshinUser);
var genshinDailyNote = await client.Genshin.FetchDailyNote(genshinUser);

// Work with Honkai Star Rail
var starRailUser = new StarRailUser(700378086);
var starRailStats = await client.StarRail.FetchStarRailStats(starRailUser);
var starRailDailyNote = await client.StarRail.FetchDailyNote(starRailUser);

// Work with Zenless Zone Zero (when available)
var zenlessUser = new ZenlessUser(800000000);
var zenlessStats = await client.Zenless.FetchZenlessStats(zenlessUser);
```

### Custom HttpClient Configuration

```csharp
// Create custom HttpClient with timeout
var httpClient = new HttpClient();
httpClient.Timeout = TimeSpan.FromSeconds(30);

// Create custom client data
var clientData = new ClientData
{
    HttpClient = httpClient,
    Language = "ru-ru",
    UserAgent = "Custom User Agent"
};

// Use with root client
using var client = HuTaoClient.Create(cookie, clientData);
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE.txt) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Disclaimer

This library is not affiliated with miHoYo/HoYoverse. Use at your own risk and in accordance with HoyoLab's Terms of Service.
