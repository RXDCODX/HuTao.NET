using HuTao.NET.GI.Models;
using HuTao.NET.GI.Models.HoYoLab;

namespace HuTao.NET.GI;

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
