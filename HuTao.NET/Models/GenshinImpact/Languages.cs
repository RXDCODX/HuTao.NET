﻿using System.Text.Json.Serialization;

namespace HuTao.NET.GI.Models.GenshinImpact;

public class Languages : IHoyoLab
{
    public int retcode { get; set; }
    public string? message { get; set; }
    [JsonPropertyName("data")]
    public LanguagesData? Data { get; set; }
}

public class LanguagesData
{
    [JsonPropertyName("langs")]
    public LangData[]? Langs { get; set; }
}

public class LangData
{
    [JsonPropertyName("alias")]
    public string[]? Alias { get; set; }

    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}
