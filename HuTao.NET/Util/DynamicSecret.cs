using System.Security.Cryptography;
using System.Text;

namespace HuTao.NET.Util;

internal class DynamicSecret
{
    private static readonly string SALT_OVERSEA = "6s25p5ox5y14umn1p61aqyyvbvvl3lrt";

    //本国API用 未使用
    //private readonly static string SALT_CHINESE = "xV8v4Qu54lUKrEYFZkJhB8cuOh9Asafs";

    private static readonly string TEXT =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    internal static string GenerateDynamicSecret()
    {
        var t = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        var r = GenerateRandomString(6);
        var h = ComputeMD5Hash($"salt={SALT_OVERSEA}&t={t}&r={r}");
        return $"{t},{r},{h}";
    }

    private static string GenerateRandomString(int n)
    {
        var result = "";
        var random = new Random();

        for (var i = 0; i < n; i++)
        {
            result += TEXT[random.Next(TEXT.Length)];
        }

        return result;
    }

    private static string ComputeMD5Hash(string str)
    {
        var md5Byte = MD5.HashData(Encoding.UTF8.GetBytes(str));
        return BitConverter.ToString(md5Byte).ToLower().Replace("-", "");
    }
}
