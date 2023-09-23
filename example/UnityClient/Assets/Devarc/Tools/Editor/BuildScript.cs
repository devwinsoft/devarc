using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Devarc;
using System.IO;
using System;
using System.Reflection;


namespace Devarc
{
    public class BuildScript
    {
        [MenuItem("Tools/Devarc/Build/BuildTables")]
        public static void BuildTables()
        {
            Table.Initailize();
            TableManager.SaveTable();
        }
    }
}