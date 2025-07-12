using System.Text.RegularExpressions;

namespace HuTao.NET.Auth;

public class Parser
{
    private static string ParseCookie(string str, string name)
    {
        var r = Regex.Match(str, name + "=[^;]+");
        return r.Value.Replace(name + "=", "");
    }

    public static Cookie CookieParser(string str)
    {
        var ltoken = ParseCookie(str, "ltoken");
        var ltuid = ParseCookie(str, "ltuid");
        return new Cookie() { LToken = ltoken, LtUid = ltuid };
    }
}
