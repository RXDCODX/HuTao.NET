using HuTao.NET.Models;
using HuTao.NET.Models.HoYoLab;

namespace HuTao.NET;

/// <summary>
/// Honkai Star Rail user representation
/// </summary>
public class StarRailUser(int uid) : BaseGameUser(uid)
{
    private const int StarRailGameID = 6;

    /// <summary>
    /// Get UID from HoyoLab user stats for Honkai Star Rail
    /// </summary>
    public static StarRailUser GetUIDFromHoyoLab(UserStats user)
    {
        return GetUIDFromHoyoLab<StarRailUser>(user, StarRailGameID);
    }

    protected override int GameId => StarRailGameID;

    protected override string GetServer(int uid)
    {
        return Util.Server.GetStarRailServer(uid);
    }
}
