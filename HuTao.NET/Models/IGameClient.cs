using HuTao.NET.GI.Models.HoYoLab;
using HuTao.NET.GI.Models.GenshinImpact;

namespace HuTao.NET.GI.Models;

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