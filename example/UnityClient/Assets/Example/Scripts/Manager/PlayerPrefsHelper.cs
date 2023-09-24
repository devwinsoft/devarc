using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Devarc;

public class PlayerPrefsHelper
{
    public static EnumPrefs<SystemLanguage> language = new EnumPrefs<SystemLanguage>("language", SystemLanguage.English);
    public static FloatPrefs maxDistance = new FloatPrefs("maxDistance", 10f);
    public static FloatMinMaxPrefs randomRange = new FloatMinMaxPrefs("randomRamge", 10f, 20f);
}

