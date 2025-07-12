using HuTao.NET.GI.Models;
using HuTao.NET.GI.Models.GenshinImpact;
using HuTao.NET.GI.Models.HonkaiStarRail;
using HuTao.NET.GI.Util;

namespace HuTao.NET.GI;

/// <summary>
/// Honkai Star Rail client for HoyoLab WebAPI
/// </summary>
public class StarRailClient : BaseGameClient
{
    /// <summary>
    /// Менеджер энергии Star Rail
    /// </summary>
    public StarRailStaminaManager StaminaManager { get; }

    /// <summary>
    /// Событие достижения максимальной энергии
    /// </summary>
    public event EventHandler<StaminaFullEventArgs>? StaminaFull;

    /// <summary>
    /// Событие получения единицы энергии
    /// </summary>
    public event EventHandler<StaminaPointReceivedEventArgs>? StaminaPointReceived;

    /// <summary>
    /// Событие изменения энергии
    /// </summary>
    public event EventHandler<StaminaChangedEventArgs>? StaminaChanged;

    internal StarRailClient(ICookie cookie)
        : base(cookie)
    {
        StaminaManager = new StarRailStaminaManager();
    }

    internal StarRailClient(ICookie cookie, ClientData data)
        : base(cookie, data)
    {
        StaminaManager = new StarRailStaminaManager();
    }

    /// <summary>
    /// Get Honkai Star Rail statistics
    /// </summary>
    public async Task<StarRailStats> FetchStarRailStats(StarRailUser user)
    {
        var url =
            _clientData.EndPoints.StarRailStats.Url + $"?server={user.Server}&role_id={user.Uid}";
        return await new Wrapper<StarRailStats>(_clientData).FetchData(url, _cookie, true);
    }

    /// <summary>
    /// Get daily note for Honkai Star Rail
    /// </summary>
    public async Task<StarRailDailyNote> FetchDailyNote(StarRailUser user)
    {
        var url =
            _clientData.EndPoints.StarRailDailyNote.Url
            + $"?server={user.Server}&role_id={user.Uid}";
        var result = await new Wrapper<StarRailDailyNote>(_clientData).FetchData(
            url,
            _cookie,
            true
        );

        // Обновляем данные в менеджере энергии
        if (result?.Data != null)
        {
            StaminaManager.UpdateDailyNoteData(result.Data);
        }

        return result ?? throw new NullReferenceException();
    }

    /// <summary>
    /// Claim daily reward for Honkai Star Rail
    /// </summary>
    public async Task<RewardData> ClaimDailyReward()
    {
        var data = new RewardData();

        var infoUrl =
            _clientData.EndPoints.StarRailRewardInfo.Url + "&lang=" + _clientData.Language;
        var homeUrl =
            _clientData.EndPoints.StarRailRewardData.Url + "&lang=" + _clientData.Language;
        var claimUrl =
            _clientData.EndPoints.StarRailRewardSign.Url + "&lang=" + _clientData.Language;

        // Get total sign days for home calculation
        var info = await FetchDynamicApi(infoUrl, false);
        var days =
            info?["data"]?["total_sign_day"]?.GetValue<int>() ?? throw new NullReferenceException();
        days--; // Subtract 1 from total days

        // Get item name and amount from home using new API structure
        var home = await FetchDynamicApi(homeUrl, false);
        var name =
            home?["data"]?["awards"]?[days]?["name"]?.GetValue<string>()
            ?? throw new NullReferenceException();
        var amount =
            home?["data"]?["awards"]?[days]?["cnt"]?.GetValue<int>()
            ?? throw new NullReferenceException();

        data.RewardName = name;
        data.Amount = amount;

        // Claim reward
        var sign = await FetchDynamicApi(claimUrl, true);
        var code = sign?["retcode"]?.GetValue<int>() ?? throw new NullReferenceException();
        if (code == 0)
        {
            data.IsSuccessed = true;
        }
        else if (code == -5003)
        {
            throw new Errors.DailyRewardAlreadyReceivedException();
        }
        else if (sign?["data"]?["gt_result"]?["is_risk"]?.GetValue<string>() == "true")
        {
            throw new Errors.HoyoLabCaptchaBlockException();
        }
        else
        {
            var message =
                sign?["message"]?.GetValue<string>() ?? throw new NullReferenceException();
            throw new Errors.HoyoLabApiBadRequestException(message, code);
        }

        return data;
    }

    /// <summary>
    /// Get Star Rail reward data using new API
    /// </summary>
    public async Task<StarRailRewardData> GetStarRailRewardData()
    {
        var url = _clientData.EndPoints.StarRailRewardData.Url + "&lang=" + _clientData.Language;
        var result = await new Wrapper<StarRailRewardData>(_clientData).FetchData(url, _cookie);

        // Инициализируем менеджер энергии с данными о наградах
        if (result?.Data != null)
        {
            StaminaManager.Initialize(result);
        }

        return result ?? throw new NullReferenceException();
    }

    /// <summary>
    /// Инициализировать менеджер энергии с данными о наградах
    /// </summary>
    public async Task InitializeStaminaManager()
    {
        try
        {
            var rewardData = await GetStarRailRewardData();
            StaminaManager.Initialize(rewardData);
        }
        catch
        {
            // Если не удалось получить данные о наградах, инициализируем без них
            StaminaManager.Initialize();
        }
    }

    /// <summary>
    /// Get stamina recovery time as TimeSpan
    /// </summary>
    public static TimeSpan GetStaminaRecoveryTime(StarRailDailyNoteData data)
    {
        return StarRailStaminaManager.GetStaminaRecoveryTime(data);
    }

    /// <summary>
    /// Get stamina full time as DateTime
    /// </summary>
    public static DateTime GetStaminaFullTime(StarRailDailyNoteData data)
    {
        return StarRailStaminaManager.GetStaminaFullTime(data);
    }

    /// <summary>
    /// Check if stamina is full
    /// </summary>
    public static bool IsStaminaFull(StarRailDailyNoteData data)
    {
        return StarRailStaminaManager.IsStaminaFull(data);
    }

    /// <summary>
    /// Get stamina percentage
    /// </summary>
    public static double GetStaminaPercentage(StarRailDailyNoteData data)
    {
        return StarRailStaminaManager.GetStaminaPercentage(data) * 100;
    }

    /// <summary>
    /// Get expedition completion time as DateTime
    /// </summary>
    public static DateTime GetExpeditionCompletionTime(StarRailExpedition expedition)
    {
        return DateTimeOffset.FromUnixTimeSeconds(expedition.FinishTimestamp).DateTime;
    }

    /// <summary>
    /// Get expedition remaining time as TimeSpan
    /// </summary>
    public static TimeSpan GetExpeditionRemainingTime(StarRailExpedition expedition)
    {
        return TimeSpan.FromSeconds(expedition.RemainingTime);
    }

    // ===== МЕТОДЫ ДЛЯ РАБОТЫ С ЭНЕРГИЕЙ =====

    /// <summary>
    /// Получить время получения следующей единицы энергии
    /// </summary>
    public static DateTime GetNextStaminaPointTime(StarRailDailyNoteData data)
    {
        return StarRailStaminaManager.GetNextStaminaPointTime(data);
    }

    /// <summary>
    /// Получить время до получения следующей единицы энергии
    /// </summary>
    public static TimeSpan GetTimeToNextStaminaPoint(StarRailDailyNoteData data)
    {
        return StarRailStaminaManager.GetTimeToNextStaminaPoint(data);
    }

    /// <summary>
    /// Получить количество единиц энергии через указанное время
    /// </summary>
    public static int GetStaminaAtTime(StarRailDailyNoteData data, TimeSpan timeSpan)
    {
        return StarRailStaminaManager.GetStaminaAtTime(data, timeSpan);
    }

    /// <summary>
    /// Получить время достижения указанного количества энергии
    /// </summary>
    public static DateTime GetTimeToReachStamina(StarRailDailyNoteData data, int targetStamina)
    {
        return StarRailStaminaManager.GetTimeToReachStamina(data, targetStamina);
    }

    /// <summary>
    /// Получить время до достижения указанного количества энергии
    /// </summary>
    public static TimeSpan GetTimeSpanToReachStamina(StarRailDailyNoteData data, int targetStamina)
    {
        return StarRailStaminaManager.GetTimeSpanToReachStamina(data, targetStamina);
    }

    /// <summary>
    /// Получить форматированную строку времени восстановления энергии
    /// </summary>
    public string GetStaminaRecoveryTimeString(StarRailDailyNoteData data)
    {
        return StaminaManager.GetStaminaRecoveryTimeString(data);
    }

    /// <summary>
    /// Получить форматированную строку времени до следующей единицы энергии
    /// </summary>
    public string GetNextStaminaPointTimeString(StarRailDailyNoteData data)
    {
        return StaminaManager.GetNextStaminaPointTimeString(data);
    }

    /// <summary>
    /// Получить информацию о текущем состоянии энергии
    /// </summary>
    public string GetStaminaInfo(StarRailDailyNoteData data)
    {
        return StaminaManager.GetStaminaInfo(data);
    }

    /// <summary>
    /// Получить прогноз энергии на указанное время
    /// </summary>
    public int GetStaminaForecast(StarRailDailyNoteData data, DateTime targetTime)
    {
        return StaminaManager.GetStaminaForecast(data, targetTime);
    }

    /// <summary>
    /// Получить рекомендации по использованию энергии
    /// </summary>
    public List<string> GetStaminaRecommendations(StarRailDailyNoteData data)
    {
        return StaminaManager.GetStaminaRecommendations(data);
    }

    // ===== МЕТОДЫ ДЛЯ РАБОТЫ С СОБЫТИЯМИ =====

    /// <summary>
    /// Вызвать событие достижения максимальной энергии
    /// </summary>
    protected virtual void OnStaminaFull(StarRailDailyNoteData data)
    {
        if (StarRailStaminaManager.IsStaminaFull(data))
        {
            var fullTime = StarRailStaminaManager.GetStaminaFullTime(data);
            var recoveryTime = StarRailStaminaManager.GetStaminaRecoveryTime(data);
            var args = new StaminaFullEventArgs(fullTime, data.MaxStamina, recoveryTime);
            StaminaFull?.Invoke(this, args);
        }
    }

    /// <summary>
    /// Вызвать событие получения единицы энергии
    /// </summary>
    protected virtual void OnStaminaPointReceived(StarRailDailyNoteData data)
    {
        var nextPointTime = StarRailStaminaManager.GetNextStaminaPointTime(data);
        var timeToNextPoint = StarRailStaminaManager.GetTimeToNextStaminaPoint(data);
        var newStaminaValue = Math.Min(data.CurrentStamina + 1, data.MaxStamina);

        var args = new StaminaPointReceivedEventArgs(
            nextPointTime,
            newStaminaValue,
            data.MaxStamina,
            timeToNextPoint
        );
        StaminaPointReceived?.Invoke(this, args);
    }

    /// <summary>
    /// Вызвать событие изменения энергии
    /// </summary>
    protected virtual void OnStaminaChanged(int previousStamina, StarRailDailyNoteData data)
    {
        var args = new StaminaChangedEventArgs(
            previousStamina,
            data.CurrentStamina,
            data.MaxStamina,
            DateTime.Now
        );
        StaminaChanged?.Invoke(this, args);
    }

    /// <summary>
    /// Проверить и вызвать события энергии
    /// </summary>
    public void CheckAndTriggerStaminaEvents(
        StarRailDailyNoteData data,
        int? previousStamina = null
    )
    {
        // Проверяем достижение максимальной энергии
        OnStaminaFull(data);

        // Проверяем получение единицы энергии (если есть предыдущее значение)
        if (previousStamina.HasValue && data.CurrentStamina > previousStamina.Value)
        {
            OnStaminaPointReceived(data);
        }

        // Вызываем событие изменения энергии
        if (previousStamina.HasValue)
        {
            OnStaminaChanged(previousStamina.Value, data);
        }
    }

    protected override string GetGameBiz()
    {
        return "hkrpg_global";
    }

    protected override int GetGameId()
    {
        return 6; // Honkai Star Rail game ID
    }
}
