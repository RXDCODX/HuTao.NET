using HuTao.NET.Models.GenshinImpact;
using HuTao.NET.Models.HoYoLab;

namespace HuTao.NET.Models;

/// <summary>
/// Базовый интерфейс для всех игровых клиентов
/// </summary>
public interface IGameClient
{
    /// <summary>
    /// Получить статистику пользователя
    /// </summary>
    Task<UserStats> FetchUserStats(string? uid = null);

    /// <summary>
    /// Получить информацию об аккаунте пользователя
    /// </summary>
    Task<UserAccountInfo> GetUserAccountInfoByLToken();

    /// <summary>
    /// Получить роли пользователя
    /// </summary>
    Task<GameRoles> GetGameRoles(bool isGameOnly = true, string region = "");
}
