using UnityEngine;
using Devarc;

public class PlayerPrefsManager

{
    public static EnumPrefs<SystemLanguage> language = new EnumPrefs<SystemLanguage>("language", SystemLanguage.English);
    public static FloatPrefs maxDistance = new FloatPrefs("maxDistance", 10f);
    public static FloatMinMaxPrefs randomRange = new FloatMinMaxPrefs("randomRamge", 10f, 20f);
}

