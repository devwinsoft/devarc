using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum LanguageType
{
    Default,
    Chinese,
    English,
    Janpanese,
    Korean,
}


public static class LStringExt
{
    public static LanguageType ToLanguageType(this string value)
    {
        LanguageType result;
        System.Enum.TryParse(value, out result);
        return result;
    }

    public static string ToISO639_2(this LanguageType lang)
    {
        switch (lang)
        {
            case LanguageType.Chinese:
                return "zho";
            case LanguageType.Janpanese:
                return "jpn";
            case LanguageType.English:
                return "eng";
            case LanguageType.Default:
            case LanguageType.Korean:
            default:
                return "kor";
        }
    }
}