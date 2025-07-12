namespace HuTao.NET.Tests;

/// <summary>
/// Тестовые данные для проверки работы API
/// </summary>
public static class TestData
{
    /// <summary>
    /// Создает тестовый CookieV2 с данными пользователя
    /// </summary>
    public static CookieV2 CreateTestCookie()
    {
        return new CookieV2
        {
            LtMidV2 = "1vw187ejtb_hy",
            LTokenV2 =
                "v2_CAISDGNpZWJod3pwcnBxOBokYjExZGNlOWQtNTczYS00MGQzLThiMmQtZWI3OGQ1MDE2YTFmILKZyLkGKLmNgvsGMMHN2ZMBQgxoa3JwZ19nbG9iYWw.sgwyZwAAAAAB.MEQCIALHwNYppKlMXrPZhvZRaWzkU7iNSY89gTI1ljDoH0rEAiATCp4AqmPGTzS0UIVDhwgRPYJEznKg65csAcOrxmVyQA",
            LtUidV2 = "309749441",
        };
    }

    /// <summary>
    /// Тестовый UID для Honkai Star Rail (Asia server)
    /// </summary>
    public static readonly int TestStarRailUid = 800000000; // Пример UID для Asia сервера

    /// <summary>
    /// Тестовый UID для Genshin Impact (Asia server)
    /// </summary>
    public static readonly int TestGenshinUid = 800000000; // Пример UID для Asia сервера
}
