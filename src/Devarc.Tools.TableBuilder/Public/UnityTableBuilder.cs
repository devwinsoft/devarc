using System;
using System.IO;

namespace Devarc
{
    public class UnityTableBuilder : BaseTableBuilder
    {
        public void Build(SettingConfig cfg, string inputPath)
        {
            var doc = open(inputPath);
            var fileName = Path.GetFileNameWithoutExtension(inputPath);
            var tableCodePath = $"{fileName}.Table.cs";

            for (int i = 0; i < doc.NumberOfSheets; i++)
            {
                var sheet = doc.GetSheetAt(i);
                readHeader(sheet);
            }

            using (var sw = File.CreateText(tableCodePath))
            {
                sw.WriteLine("using System;");
                sw.WriteLine("using System.IO;");
                sw.WriteLine("using UnityEngine;");
                sw.WriteLine("#if UNITY_EDITOR");
                sw.WriteLine("using UnityEditor;");
                sw.WriteLine("#endif");
                sw.WriteLine("using MessagePack;");
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
                    sw.WriteLine($"\t\t\tTableManager.RegisterLoadTableBin(\"{info.SheetName}\", (data, options) =>");
                    sw.WriteLine("\t\t\t{");
                    sw.WriteLine("\t\t\t\tLoadBin(data, options);");
                    sw.WriteLine("\t\t\t});");
                    sw.WriteLine($"\t\t\tTableManager.RegisterLoadTableJson(\"{info.SheetName}\", (textAsset) =>");
                    sw.WriteLine("\t\t\t{");
                    sw.WriteLine("\t\t\t\tLoadJson(textAsset.text);");
                    sw.WriteLine("\t\t\t});");
                    sw.WriteLine($"\t\t\tTableManager.RegisterSaveTable(\"{info.SheetName}\", (textAsset, isBundle, lang) =>");
                    sw.WriteLine("\t\t\t{");
                    sw.WriteLine("\t\t\t\tSaveBin(textAsset, isBundle, lang);");
                    sw.WriteLine("\t\t\t});");
                    sw.WriteLine($"\t\t\tTableManager.RegisterUnloadTable(\"{info.SheetName}\", () =>");
                    sw.WriteLine("\t\t\t{");
                    sw.WriteLine("\t\t\t\tClear();");
                    sw.WriteLine("\t\t\t});");
                    sw.WriteLine("\t\t}");
                    sw.WriteLine("\t\tpublic void LoadBin(byte[] data, MessagePackSerializerOptions options)");
                    sw.WriteLine("\t\t{");
                    sw.WriteLine("\t\t\tInitLoad(data);");
                    sw.WriteLine("\t\t\tint count = ReadInt();");
                    sw.WriteLine("\t\t\tfor (int i = 0; i < count; i++)");
                    sw.WriteLine("\t\t\t{");
                    sw.WriteLine("\t\t\t\tint size = ReadInt();");
                    sw.WriteLine("\t\t\t\tvar temp = ReadBytes(size);");
                    sw.WriteLine($"\t\t\t\tvar obj = MessagePackSerializer.Deserialize<{info.SheetName}>(temp, options);");
                    sw.WriteLine("\t\t\t\tAdd(obj.GetKey(), obj);");
                    sw.WriteLine("\t\t\t}");
                    sw.WriteLine("\t\t}");
                    sw.WriteLine("\t\tpublic void SaveBin(TextAsset textAsset, bool isBundle, SystemLanguage lang)");
                    sw.WriteLine("\t\t{");
                    sw.WriteLine("#if UNITY_EDITOR");
                    sw.WriteLine("\t\t\tClear();");
                    sw.WriteLine("\t\t\tLoadJson(textAsset.text);");
                    sw.WriteLine("\t\t\tvar saveAsset = new TextAsset(Convert.ToBase64String(GetBytes()));");
                    sw.WriteLine($"\t\t\tvar filePath = Path.Combine(DEV_Settings.GetTablePath(isBundle, TableFormatType.BIN), \"{info.SheetName}.asset\");");
                    sw.WriteLine("\t\t\tAssetDatabase.CreateAsset(saveAsset, filePath);");
                    sw.WriteLine("#endif");
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

            if (string.IsNullOrEmpty(cfg.client_script) == false)
            {
                var folderName = Path.Combine(cfg.client_script, "Editor");
                if (Directory.Exists(folderName) == false)
                    Directory.CreateDirectory(folderName);
                var destPath = Path.Combine(folderName, tableCodePath);
                File.Copy(tableCodePath, destPath, true);
                Console.WriteLine($"Copy to: {destPath}");
            }
            File.Delete(tableCodePath);
            
            foreach (var info in mHeaderList)
            {
                if (info.IsDataSheet)
                    continue;

                var editorCodeNameEx = $"{info.SheetName}_ID.Editor.cs";
                using (var sw = File.CreateText(editorCodeNameEx))
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
                    sw.WriteLine($"\t\t\tforeach (var textAsset in AssetManager.FindAssets<TextAsset>(\"{info.SheetName}\", DEV_Settings.GetTablePath(true, TableFormatType.JSON)))");
                    sw.WriteLine("\t\t\t{");
                    sw.WriteLine($"\t\t\t\tTable.{info.SheetName}.LoadJson(textAsset.text);");
                    sw.WriteLine("\t\t\t}");
                    sw.WriteLine($"\t\t\tforeach (var textAsset in AssetManager.FindAssets<TextAsset>(\"{info.SheetName}\", DEV_Settings.GetTablePath(false, TableFormatType.JSON)))");
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
                Console.WriteLine($"Generate file: {editorCodeNameEx}");
                
                if (string.IsNullOrEmpty(cfg.client_script) == false)
                {
                    var folderName = Path.Combine(cfg.client_script, "Editor");
                    if (Directory.Exists(folderName) == false)
                        Directory.CreateDirectory(folderName);
                    var destPath = Path.Combine(folderName, editorCodeNameEx);
                    File.Copy(editorCodeNameEx, destPath, true);
                    Console.WriteLine($"Copy to: {destPath}");
                }
                File.Delete(editorCodeNameEx);
            }
        }
    }
}
