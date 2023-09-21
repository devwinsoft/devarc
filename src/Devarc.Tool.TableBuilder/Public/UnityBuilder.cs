using System;
using System.IO;

namespace Devarc
{
    public class UnityBuilder : BaseBuilder
    {
        public void Build(string inputPath)
        {
            var doc = open(inputPath);
            var workingDir = Path.GetFullPath(".\\");
            var fileName = Path.GetFileNameWithoutExtension(inputPath);
            var tableCodePath = Path.Combine(workingDir, $"{fileName}.Table.cs");

            for (int i = 0; i < doc.NumberOfSheets; i++)
            {
                var sheet = doc.GetSheetAt(i);
                readHeader(sheet);
            }

            using (var sw = File.CreateText(tableCodePath))
            {
                sw.WriteLine("namespace Devarc");
                sw.WriteLine("{");
                foreach (var info in mHeaderList)
                {
                    if (info.IsDataSheet)
                        continue;
                    sw.WriteLine($"\tpublic class _{info.SheetName}_TABLE : TableData<{info.SheetName}, _{info.SheetName}, {info.KeyTypeName}>");
                    sw.WriteLine("\t{");
                    sw.WriteLine($"\t\tpublic _{info.SheetName}_TABLE()");
                    sw.WriteLine("\t\t{");
                    sw.WriteLine($"\t\t\tTableManager.Instance.registerLoadTableCallback(\"{info.SheetName}\", (textAsset) =>");
                    sw.WriteLine("\t\t\t{");
                    sw.WriteLine("\t\t\t\tLoadJson(textAsset.text);");
                    sw.WriteLine("\t\t\t});");
                    sw.WriteLine($"\t\t\tTableManager.Instance.registerUnloadTableCallback(\"{info.SheetName}\", () =>");
                    sw.WriteLine("\t\t\t{");
                    sw.WriteLine("\t\t\t\tClear();");
                    sw.WriteLine("\t\t\t});");
                    sw.WriteLine("\t\t}");
                    sw.WriteLine("\t}");
                }
                sw.WriteLine($"\tpublic partial class Table");
                sw.WriteLine("\t{");
                foreach (var info in mHeaderList)
                {
                    if (info.IsDataSheet)
                        continue;
                    sw.WriteLine($"\t\tpublic static _{info.SheetName}_TABLE {info.SheetName} = new _{info.SheetName}_TABLE();");
                }
                sw.WriteLine("\t}");
                sw.WriteLine("");

                foreach (var info in mHeaderList)
                {
                    if (info.IsDataSheet)
                        continue;

                    sw.WriteLine($"\t[System.Serializable]");
                    sw.WriteLine($"\tpublic class {info.SheetName}_ID");
                    sw.WriteLine("\t{");
                    sw.WriteLine("\t\tpublic string Value = string.Empty;");
                    sw.WriteLine($"\t\tpublic static implicit operator string({info.SheetName}_ID obj)");
                    sw.WriteLine("\t\t{");
                    sw.WriteLine("\t\t\tif (obj == null) return string.Empty;");
                    sw.WriteLine("\t\t\treturn obj.Value;");
                    sw.WriteLine("\t\t}");
                    sw.WriteLine($"\t\tpublic static implicit operator {info.SheetName}_ID(string value)");
                    sw.WriteLine("\t\t{");
                    sw.WriteLine($"\t\t\t{info.SheetName}_ID obj = new {info.SheetName}_ID();");
                    sw.WriteLine("\t\t\tobj.Value = value;");
                    sw.WriteLine("\t\t\treturn obj;");
                    sw.WriteLine("\t\t}");
                    sw.WriteLine("\t}");
                    sw.WriteLine("");
                }

                sw.WriteLine($"\tpublic static class {fileName}_Extension");
                sw.WriteLine("\t{");
                foreach (var info in mHeaderList)
                {
                    if (info.IsDataSheet)
                        continue;
                    sw.WriteLine($"\t\tpublic static bool IsValid(this {info.SheetName}_ID obj)");
                    sw.WriteLine("\t\t{");
                    sw.WriteLine("\t\t\treturn obj != null && !string.IsNullOrEmpty(obj.Value);");
                    sw.WriteLine("\t\t}");
                }
                sw.WriteLine("\t}");

                sw.WriteLine("}"); // end of name space
                sw.Close();
            }
            Console.WriteLine($"Generate file: {tableCodePath}");


            foreach (var info in mHeaderList)
            {
                if (info.IsDataSheet)
                    continue;

                var editorCodePath = Path.Combine(workingDir, $"{info.SheetName}_ID.Editor.cs");
                using (var sw = File.CreateText(editorCodePath))
                {
                    sw.WriteLine("using UnityEngine;");
                    sw.WriteLine("using UnityEditor;");
                    sw.WriteLine("using System.Collections;");
                    sw.WriteLine("");
                    sw.WriteLine("namespace Devarc");
                    sw.WriteLine("{");
                    sw.WriteLine($"\t[CustomPropertyDrawer(typeof({info.SheetName}_ID))]");
                    sw.WriteLine($"\tpublic class {info.SheetName}_ID_Drawer : EditorID_Drawer<{info.SheetName}>");
                    sw.WriteLine("\t{");
                    sw.WriteLine($"\t\tprotected override EditorID_Selector<{info.SheetName}> getSelector()");
                    sw.WriteLine("\t\t{");
                    sw.WriteLine($"\t\t\treturn {info.SheetName}_ID_Selector.Instance;");
                    sw.WriteLine("\t\t}");
                    sw.WriteLine("\t}");
                    sw.WriteLine("");
                    sw.WriteLine($"\tpublic class {info.SheetName}_ID_Selector : EditorID_Selector<{info.SheetName}>");
                    sw.WriteLine("\t{");
                    sw.WriteLine($"\t\tpublic new static EditorID_Selector<{info.SheetName}> Instance");
                    sw.WriteLine("\t\t{");
                    sw.WriteLine("\t\t\tget {");
                    sw.WriteLine("\t\t\t\tif (msInstance != null) return msInstance;");
                    sw.WriteLine($"\t\t\t\tmsInstance = ScriptableWizard.DisplayWizard<{info.SheetName}_ID_Selector>(\"Select {info.SheetName}_ID\");");
                    sw.WriteLine("\t\t\t\treturn msInstance;");
                    sw.WriteLine("\t\t\t}");
                    sw.WriteLine("\t\t}");
                    sw.WriteLine("");
                    sw.WriteLine("\t\tpublic override void Reload()");
                    sw.WriteLine("\t\t{");
                    sw.WriteLine($"\t\t\tTable.{info.SheetName}.Clear();");
                    sw.WriteLine($"\t\t\tforeach (var textAsset in AssetManager.FindAssets<TextAsset>(\"{info.SheetName}\", DEV_Settings.GetTable_BundlePath()))");
                    sw.WriteLine("\t\t\t{");
                    sw.WriteLine($"\t\t\t\tTable.{info.SheetName}.LoadJson(textAsset.text);");
                    sw.WriteLine("\t\t\t}");
                    sw.WriteLine($"\t\t\tforeach (var textAsset in AssetManager.FindAssets<TextAsset>(\"{info.SheetName}\", DEV_Settings.GetTable_ResourcePath()))");
                    sw.WriteLine("\t\t\t{");
                    sw.WriteLine($"\t\t\t\tTable.{info.SheetName}.LoadJson(textAsset.text);");
                    sw.WriteLine("\t\t\t}");
                    if (string.IsNullOrEmpty(info.DisplayName))
                    {
                        sw.WriteLine($"\t\t\tforeach (var obj in Table.{info.SheetName}.List) add(obj.{info.KeyFieldName});");
                    }
                    else if (info.ShowKey)
                    {
                        sw.WriteLine($"\t\t\tforeach (var obj in Table.{info.SheetName}.List) add($\"{{obj.{info.KeyFieldName}}}:{{obj.{info.DisplayName}}}\");");
                    }
                    else
                    {
                        sw.WriteLine($"\t\t\tforeach (var obj in Table.{info.SheetName}.List) add(obj.{info.DisplayName});");
                    }
                    sw.WriteLine("\t\t}");
                    sw.WriteLine("\t}");
                    sw.WriteLine("}"); // end of name space
                    sw.Close();
                }
                Console.WriteLine($"Generate file: {editorCodePath}");
            }
        }
    }
}
