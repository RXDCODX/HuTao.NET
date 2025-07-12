using HuTao.NET.GI.Models;
using HuTao.NET.GI.Models.HoYoLab;

namespace HuTao.NET.GI;

/// <summary>
/// Zenless Zone Zero user representation
/// </summary>
public class ZenlessUser(int uid) : BaseGameUser(uid)
{
    private const int ZenlessGameID = 8; // Placeholder ID

    /// <summary>
    /// Get UID from HoyoLab user stats for Zenless Zone Zero
    /// </summary>
    public static ZenlessUser GetUIDFromHoyoLab(UserStats user)
    {
        return GetUIDFromHoyoLab<ZenlessUser>(user, ZenlessGameID);
    }

    protected override int GameId => ZenlessGameID;

    protected override string GetServer(int uid)
    {
        return Util.Server.GetZenlessServer(uid);
    }
}
