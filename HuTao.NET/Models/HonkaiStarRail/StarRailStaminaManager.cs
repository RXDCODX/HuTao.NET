namespace HuTao.NET.Models.HonkaiStarRail;

/// <summary>
/// Менеджер энергии для Honkai Star Rail
/// </summary>
public class StarRailStaminaManager
{
    /// <summary>
    /// Скорость восстановления энергии (секунды на единицу)
    /// </summary>
    public const int StaminaRecoverySeconds = 360; // 6 минут

    /// <summary>
    /// Максимальное количество энергии
    /// </summary>
    public const int MaxStamina = 300;

    /// <summary>
    /// Данные о наградах (для расчета времени восстановления)
    /// </summary>
    public StarRailRewardData? RewardData { get; private set; }

    /// <summary>
    /// Последние данные о ежедневной заметке
    /// </summary>
    public StarRailDailyNoteData? LastDailyNoteData { get; private set; }

    /// <summary>
    /// Время последнего обновления данных
    /// </summary>
    public DateTime LastUpdateTime { get; private set; }

    internal StarRailStaminaManager(StarRailRewardData? rewardData = null)
    {
        RewardData = rewardData;
        LastUpdateTime = DateTime.Now;
    }

    /// <summary>
    /// Инициализировать менеджер энергии
    /// </summary>
    /// <param name="rewardData">Данные о наградах</param>
    public void Initialize(StarRailRewardData? rewardData = null)
    {
        RewardData = rewardData;
        LastUpdateTime = DateTime.Now;
    }

    /// <summary>
    /// Обновить данные ежедневной заметки
    /// </summary>
    /// <param name="dailyNoteData">Новые данные</param>
    public void UpdateDailyNoteData(StarRailDailyNoteData dailyNoteData)
    {
        LastDailyNoteData = dailyNoteData;
        LastUpdateTime = DateTime.Now;
    }

    /// <summary>
    /// Получить время полного восстановления энергии
    /// </summary>
    /// <param name="data">Данные ежедневной заметки</param>
    /// <returns>Время полного восстановления энергии</returns>
    public static DateTime GetStaminaFullTime(StarRailDailyNoteData data)
    {
        return DateTimeOffset.FromUnixTimeSeconds(data.StaminaFullTimestamp).DateTime;
    }

    /// <summary>
    /// Получить время до полного восстановления энергии
    /// </summary>
    /// <param name="data">Данные ежедневной заметки</param>
    /// <returns>Время до полного восстановления</returns>
    public static TimeSpan GetStaminaRecoveryTime(StarRailDailyNoteData data)
    {
        return TimeSpan.FromSeconds(data.StaminaRecoverTime);
    }

    /// <summary>
    /// Проверить, заполнена ли энергия
    /// </summary>
    /// <param name="data">Данные ежедневной заметки</param>
    /// <returns>True если энергия заполнена</returns>
    public static bool IsStaminaFull(StarRailDailyNoteData data) =>
        data.CurrentStamina >= data.MaxStamina;

    /// <summary>
    /// Получить процент заполнения энергии
    /// </summary>
    /// <param name="data">Данные ежедневной заметки</param>
    /// <returns>Процент заполнения (0.0 - 1.0)</returns>
    public static double GetStaminaPercentage(StarRailDailyNoteData data)
    {
        return (double)data.CurrentStamina / data.MaxStamina;
    }

    /// <summary>
    /// Получить время получения следующей единицы энергии
    /// </summary>
    /// <param name="data">Данные ежедневной заметки</param>
    /// <returns>Время получения следующей единицы энергии</returns>
    public static DateTime GetNextStaminaPointTime(StarRailDailyNoteData data)
    {
        var currentTime = DateTimeOffset.FromUnixTimeSeconds(data.CurrentTimestamp).DateTime;
        var timeToNextPoint =
            StaminaRecoverySeconds - (data.StaminaRecoverTime % StaminaRecoverySeconds);

        return currentTime.AddSeconds(timeToNextPoint);
    }

    /// <summary>
    /// Получить время до получения следующей единицы энергии
    /// </summary>
    /// <param name="data">Данные ежедневной заметки</param>
    /// <returns>Время до следующей единицы энергии</returns>
    public static TimeSpan GetTimeToNextStaminaPoint(StarRailDailyNoteData data)
    {
        // Если энергия уже заполнена, время до следующей единицы равно 0
        if (IsStaminaFull(data))
        {
            return TimeSpan.Zero;
        }

        var timeToNextPoint =
            StaminaRecoverySeconds - (data.StaminaRecoverTime % StaminaRecoverySeconds);
        return TimeSpan.FromSeconds(timeToNextPoint);
    }

    /// <summary>
    /// Получить количество единиц энергии, которые будут восстановлены через указанное время
    /// </summary>
    /// <param name="data">Данные ежедневной заметки</param>
    /// <param name="timeSpan">Время для расчета</param>
    /// <returns>Количество единиц энергии</returns>
    public static int GetStaminaAtTime(StarRailDailyNoteData data, TimeSpan timeSpan)
    {
        var totalSeconds = timeSpan.TotalSeconds;
        var staminaToRecover = (int)(totalSeconds / StaminaRecoverySeconds);

        return Math.Min(data.CurrentStamina + staminaToRecover, data.MaxStamina);
    }

    /// <summary>
    /// Получить время, когда энергия достигнет указанного значения
    /// </summary>
    /// <param name="data">Данные ежедневной заметки</param>
    /// <param name="targetStamina">Целевое количество энергии</param>
    /// <returns>Время достижения целевого значения</returns>
    public static DateTime GetTimeToReachStamina(StarRailDailyNoteData data, int targetStamina)
    {
        if (targetStamina <= data.CurrentStamina)
        {
            return DateTimeOffset.FromUnixTimeSeconds(data.CurrentTimestamp).DateTime;
        }

        if (targetStamina > data.MaxStamina)
        {
            targetStamina = data.MaxStamina;
        }

        var staminaNeeded = targetStamina - data.CurrentStamina;
        var timeNeeded = staminaNeeded * StaminaRecoverySeconds;

        var currentTime = DateTimeOffset.FromUnixTimeSeconds(data.CurrentTimestamp).DateTime;
        return currentTime.AddSeconds(timeNeeded);
    }

    /// <summary>
    /// Получить время до достижения указанного количества энергии
    /// </summary>
    /// <param name="data">Данные ежедневной заметки</param>
    /// <param name="targetStamina">Целевое количество энергии</param>
    /// <returns>Время до достижения целевого значения</returns>
    public static TimeSpan GetTimeSpanToReachStamina(StarRailDailyNoteData data, int targetStamina)
    {
        if (targetStamina <= data.CurrentStamina)
        {
            return TimeSpan.Zero;
        }

        if (targetStamina > data.MaxStamina)
        {
            targetStamina = data.MaxStamina;
        }

        var staminaNeeded = targetStamina - data.CurrentStamina;
        var timeNeeded = staminaNeeded * StaminaRecoverySeconds;

        return TimeSpan.FromSeconds(timeNeeded);
    }

    /// <summary>
    /// Получить форматированную строку времени восстановления энергии
    /// </summary>
    /// <param name="data">Данные ежедневной заметки</param>
    /// <returns>Форматированная строка времени</returns>
    public static string GetStaminaRecoveryTimeString(StarRailDailyNoteData data)
    {
        var timeSpan = GetStaminaRecoveryTime(data);

        return timeSpan.TotalHours >= 1
            ? $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}"
            : $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }

    /// <summary>
    /// Получить форматированную строку времени до следующей единицы энергии
    /// </summary>
    /// <param name="data">Данные ежедневной заметки</param>
    /// <returns>Форматированная строка времени</returns>
    public static string GetNextStaminaPointTimeString(StarRailDailyNoteData data)
    {
        var timeSpan = GetTimeToNextStaminaPoint(data);
        return $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }

    /// <summary>
    /// Получить информацию о текущем состоянии энергии
    /// </summary>
    /// <param name="data">Данные ежедневной заметки</param>
    /// <returns>Строка с информацией о энергии</returns>
    public static string GetStaminaInfo(StarRailDailyNoteData data)
    {
        var percentage = GetStaminaPercentage(data) * 100;
        var isFull = IsStaminaFull(data);
        var recoveryTime = GetStaminaRecoveryTimeString(data);
        var nextPoint = GetNextStaminaPointTimeString(data);

        return $"Энергия: {data.CurrentStamina}/{data.MaxStamina} ({percentage:F1}%) "
            + $"| Заполнена: {(isFull ? "Да" : "Нет")} "
            + $"| До полного восстановления: {recoveryTime} "
            + $"| Следующая единица через: {nextPoint}";
    }

    /// <summary>
    /// Получить прогноз энергии на указанное время
    /// </summary>
    /// <param name="data">Данные ежедневной заметки</param>
    /// <param name="targetTime">Целевое время</param>
    /// <returns>Прогноз энергии</returns>
    public static int GetStaminaForecast(StarRailDailyNoteData data, DateTime targetTime)
    {
        var currentTime = DateTimeOffset.FromUnixTimeSeconds(data.CurrentTimestamp).DateTime;
        var timeSpan = targetTime - currentTime;

        return timeSpan <= TimeSpan.Zero ? data.CurrentStamina : GetStaminaAtTime(data, timeSpan);
    }

    /// <summary>
    /// Получить время до оптимального использования энергии
    /// </summary>
    /// <param name="data">Данные ежедневной заметки</param>
    /// <param name="targetStamina">Целевое количество энергии для использования</param>
    /// <returns>Время до оптимального использования</returns>
    public static DateTime GetOptimalUsageTime(StarRailDailyNoteData data, int targetStamina)
    {
        // Если энергии достаточно, можно использовать сейчас
        if (data.CurrentStamina >= targetStamina)
        {
            return DateTimeOffset.FromUnixTimeSeconds(data.CurrentTimestamp).DateTime;
        }

        // Иначе ждем восстановления
        return GetTimeToReachStamina(data, targetStamina);
    }

    /// <summary>
    /// Проверить, нужно ли использовать энергию сейчас
    /// </summary>
    /// <param name="data">Данные ежедневной заметки</param>
    /// <param name="threshold">Порог для использования (по умолчанию 80% от максимума)</param>
    /// <returns>True если рекомендуется использовать энергию</returns>
    public static bool ShouldUseStaminaNow(StarRailDailyNoteData data, double threshold = 0.8)
    {
        var percentage = GetStaminaPercentage(data);
        return percentage >= threshold;
    }

    /// <summary>
    /// Получить рекомендации по использованию энергии
    /// </summary>
    /// <param name="data">Данные ежедневной заметки</param>
    /// <returns>Список рекомендаций</returns>
    public static List<string> GetStaminaRecommendations(StarRailDailyNoteData data)
    {
        var recommendations = new List<string>();

        if (IsStaminaFull(data))
        {
            recommendations.Add("⚠️ Энергия заполнена! Рекомендуется использовать её.");
        }
        else if (ShouldUseStaminaNow(data))
        {
            recommendations.Add(
                "💡 Энергия близка к максимуму. Рассмотрите возможность использования."
            );
        }

        var timeToFull = GetStaminaRecoveryTime(data);
        if (timeToFull.TotalHours > 20)
        {
            recommendations.Add("⏰ До полного восстановления энергии более 20 часов.");
        }

        var nextPoint = GetTimeToNextStaminaPoint(data);
        if (nextPoint.TotalMinutes < 10)
        {
            recommendations.Add("🔄 Следующая единица энергии скоро будет получена.");
        }

        return recommendations;
    }
}
