using System.Text.Json.Serialization;

namespace HuTao.NET.GI.Models.HonkaiStarRail;

/// <summary>
/// Аргументы события достижения максимальной энергии
/// </summary>
public sealed class StaminaFullEventArgs(DateTime fullTime, int maxStamina, TimeSpan recoveryTime)
    : EventArgs
{
    /// <summary>
    /// Время достижения максимальной энергии
    /// </summary>
    public DateTime FullTime { get; } = fullTime;

    /// <summary>
    /// Максимальное количество энергии
    /// </summary>
    public int MaxStamina { get; } = maxStamina;

    /// <summary>
    /// Время восстановления до максимума
    /// </summary>
    public TimeSpan RecoveryTime { get; } = recoveryTime;
}

/// <summary>
/// Аргументы события получения единицы энергии
/// </summary>
public sealed class StaminaPointReceivedEventArgs(
    DateTime pointTime,
    int newStaminaValue,
    int maxStamina,
    TimeSpan timeToNextPoint
) : EventArgs
{
    /// <summary>
    /// Время получения единицы энергии
    /// </summary>
    public DateTime PointTime { get; } = pointTime;

    /// <summary>
    /// Количество энергии после получения единицы
    /// </summary>
    public int NewStaminaValue { get; } = newStaminaValue;

    /// <summary>
    /// Максимальное количество энергии
    /// </summary>
    public int MaxStamina { get; } = maxStamina;

    /// <summary>
    /// Время до следующей единицы энергии
    /// </summary>
    public TimeSpan TimeToNextPoint { get; } = timeToNextPoint;

    /// <summary>
    /// Процент заполнения энергии после получения единицы
    /// </summary>
    public double StaminaPercentage { get; } = (double)newStaminaValue / maxStamina;
}

/// <summary>
/// Аргументы события изменения энергии
/// </summary>
public sealed class StaminaChangedEventArgs(
    int previousStamina,
    int currentStamina,
    int maxStamina,
    DateTime changeTime
) : EventArgs
{
    /// <summary>
    /// Предыдущее количество энергии
    /// </summary>
    public int PreviousStamina { get; } = previousStamina;

    /// <summary>
    /// Текущее количество энергии
    /// </summary>
    public int CurrentStamina { get; } = currentStamina;

    /// <summary>
    /// Максимальное количество энергии
    /// </summary>
    public int MaxStamina { get; } = maxStamina;

    /// <summary>
    /// Изменение энергии (может быть отрицательным при трате)
    /// </summary>
    public int StaminaChange { get; } = currentStamina - previousStamina;

    /// <summary>
    /// Процент заполнения энергии
    /// </summary>
    public double StaminaPercentage { get; } = (double)currentStamina / maxStamina;

    /// <summary>
    /// Время изменения
    /// </summary>
    public DateTime ChangeTime { get; } = changeTime;
}

/// <summary>
/// Вспомогательные методы для работы с энергией в Honkai Star Rail
/// </summary>
public static class StarRailStaminaEvents
{
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
    public static bool IsStaminaFull(StarRailDailyNoteData data)
    {
        return data.CurrentStamina >= data.MaxStamina;
    }

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
        // В Star Rail энергия восстанавливается каждые 6 минут (360 секунд)
        const int staminaRecoverySeconds = 360;

        var currentTime = DateTimeOffset.FromUnixTimeSeconds(data.CurrentTimestamp).DateTime;
        var timeToNextPoint =
            staminaRecoverySeconds - (data.StaminaRecoverTime % staminaRecoverySeconds);

        return currentTime.AddSeconds(timeToNextPoint);
    }

    /// <summary>
    /// Получить время до получения следующей единицы энергии
    /// </summary>
    /// <param name="data">Данные ежедневной заметки</param>
    /// <returns>Время до следующей единицы энергии</returns>
    public static TimeSpan GetTimeToNextStaminaPoint(StarRailDailyNoteData data)
    {
        // В Star Rail энергия восстанавливается каждые 6 минут (360 секунд)
        const int staminaRecoverySeconds = 360;

        var timeToNextPoint =
            staminaRecoverySeconds - (data.StaminaRecoverTime % staminaRecoverySeconds);

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
        // В Star Rail энергия восстанавливается каждые 6 минут (360 секунд)
        const int staminaRecoverySeconds = 360;

        var totalSeconds = timeSpan.TotalSeconds;
        var staminaToRecover = (int)(totalSeconds / staminaRecoverySeconds);

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

        // В Star Rail энергия восстанавливается каждые 6 минут (360 секунд)
        const int staminaRecoverySeconds = 360;

        var staminaNeeded = targetStamina - data.CurrentStamina;
        var timeNeeded = staminaNeeded * staminaRecoverySeconds;

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

        // В Star Rail энергия восстанавливается каждые 6 минут (360 секунд)
        const int staminaRecoverySeconds = 360;

        var staminaNeeded = targetStamina - data.CurrentStamina;
        var timeNeeded = staminaNeeded * staminaRecoverySeconds;

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
}
