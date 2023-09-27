using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Devarc;

#if UNITY_WEBGL
public class WebGLManager : MonoSingleton<WebGLManager>
{
    [DllImport("__Internal")] private static extern void _init();
    [DllImport("__Internal")] private static extern void _openURL(string _url);
    [DllImport("__Internal")] private static extern string _stringReturnValueFunction();


    public void Refresh(bool _force = false)
    {
        Debug.Log("[WebGLManager::Refresh]");
    }
}
#endif

