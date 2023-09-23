using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BinTableBuilder : ScriptableWizard
{
    public void Build()
    {
        Type type = typeof(BinTableBuilder);

        //foreach (var finfo in type.GetFields())
        //{
        //    finfo.GetCustomAttributes(typeof(), false);
        //}
        //foreach (var pinfo in type.GetProperties())
        //{
        //    pinfo.GetCustomAttributes(typeof(), false);
        //}
    }
}
