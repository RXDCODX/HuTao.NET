using HuTao.NET.Models;
using HuTao.NET.Models.HoYoLab;

namespace HuTao.NET;

/// <summary>
/// Genshin Impact user representation
/// </summary>
public class GenshinUser(int uid) : BaseGameUser(uid)
{
    private const int GenshinGameID = 2;

    /// <summary>
    /// Get UID from HoyoLab user stats for Genshin Impact
    /// </summary>
    public static GenshinUser GetUIDFromHoyoLab(UserStats user)
    {
        return GetUIDFromHoyoLab<GenshinUser>(user, GenshinGameID);
    }

    protected override int GameId => GenshinGameID;

    protected override string GetServer(int uid)
    {
        return Util.Server.GetGenshinServer(uid);
    }
}
