using HuTao.NET.GI.Models.HoYoLab;
using HuTao.NET.GI.Util;

namespace HuTao.NET.GI.Models;

/// <summary>
/// Base class for game users
/// </summary>
public abstract class BaseGameUser
{
    public int Uid { get; }
    public string Server { get; }
    protected abstract int GameId { get; }

    protected BaseGameUser(int uid)
    {
        Uid = uid;
        Server = GetServer(uid);
    }

    /// <summary>
    /// Get UID from HoyoLab user stats
    /// </summary>
    public static T GetUIDFromHoyoLab<T>(UserStats user, int gameId)
        where T : BaseGameUser
    {
        var uid = 0;
        Parallel.ForEach(
            user.Data!.GameLists!,
            game =>
            {
                if (game.GameId == gameId)
                {
                    uid = int.Parse(game.GameUid!);
                }
            }
        );

        return uid == 0
            ? throw new Errors.AccountNotFoundException()
            : (T)Activator.CreateInstance(typeof(T), uid)!;
    }

    /// <summary>
    /// Get server based on UID
    /// </summary>
    protected abstract string GetServer(int uid);
}
