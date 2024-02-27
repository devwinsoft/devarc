using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.UserModel;
using System;
using System.IO;

namespace Devarc
{
    public class CsBuilder : BaseBuilder
    {
        public void Build(string inputPath)
        {
            var doc = open(inputPath);
            var workingDir = Path.GetFullPath(".\\");
            var fileName = Path.GetFileNameWithoutExtension(inputPath);
            var filePath = Path.Combine(workingDir, $"{fileName}.cs");

            using (var sw = File.CreateText(filePath))
            {
                sw.WriteLine("using MessagePack;");
                sw.WriteLine("");
                sw.WriteLine("namespace Devarc");
                sw.WriteLine("{");
                for (int i = 0; i < doc.NumberOfSheets; i++)
                {
                    var sheet = doc.GetSheetAt(i);
                    readHeader(sheet);

                    if (mCurrentHeader.IsDataSheet)
                        continue;
                    onReadSheet(sheet, sw);
                }
                sw.WriteLine("}");
                sw.Close();
            }
            Console.WriteLine($"Generate file: {filePath}");
        }


        string getFunctionStr(FieldData info)
        {
            string typeName = info.fieldType.ToLower();
            if (info.isList)
            {
                switch (typeName)
                {
                    case "bool":
                    case "boolean":
                        return $"GetBool({info.fieldName})";
                    case "int":
                        return $"GetIntArray({info.fieldName})";
                    case "float":
                        return $"GetFloatArray({info.fieldName})";
                    case "string":
                        return $"GetStringArray({info.fieldName})";
                    default:
                        if (info.isClass)
                            return $"GetClassArray<{info.fieldType}>({info.fieldName})";
                        else
                            return $"GetEnumArray<{info.fieldType}>({info.fieldName})";
                }
            }
            else
            {
                switch (typeName)
                {
                    case "bool":
                    case "boolean":
                        return $"GetBool({info.fieldName})";
                    case "int":
                        return $"GetInt({info.fieldName})";
                    case "float":
                        return $"GetFloat({info.fieldName})";
                    case "string":
                        return $"({info.fieldName})";
                    default:
                        if (info.isClass)
                            return $"GetClass<{info.fieldType}>({info.fieldName})";
                        else
                            return $"GetEnum<{info.fieldType}>({info.fieldName})";
                }
            }
        }

        void onReadSheet(ISheet sheet, StreamWriter sw)
        {
            sw.WriteLine("\t[System.Serializable]");
            sw.WriteLine($"\tpublic partial class RawTableData_{sheet.SheetName} : RawTableData");
            sw.WriteLine("\t{");
            for (int c = 0; c < mCurrentHeader.MaxColumn; c++)
            {
                var data = mCurrentHeader.Get(c);
                if (data == null)
                    continue;
                sw.WriteLine(string.Format("\t\tpublic {0,-20} {1};", "string", data.fieldName));
            }
            sw.WriteLine("");
            for (int c = 0; c < mCurrentHeader.MaxColumn; c++)
            {
                var data = mCurrentHeader.Get(c);
                if (data == null)
                    continue;
                if (data.isList)
                    sw.WriteLine(string.Format("\t\tpublic virtual {0,-12} get_{1}() => {2};", data.fieldType + "[]", data.fieldName, getFunctionStr(data)));
                else
                    sw.WriteLine(string.Format("\t\tpublic virtual {0,-12} get_{1}() => {2};", data.fieldType, data.fieldName, getFunctionStr(data)));
            }
            sw.WriteLine("\t}");
            sw.WriteLine("");
            sw.WriteLine("\t[System.Serializable]");
            sw.WriteLine($"\tpublic partial class _{sheet.SheetName} : RawTableData_{sheet.SheetName}");
            sw.WriteLine("\t{");
            sw.WriteLine("\t}");
            sw.WriteLine("");
            sw.WriteLine("\t[System.Serializable]");
            sw.WriteLine("\t[MessagePackObject]");
            sw.WriteLine($"\tpublic partial class {sheet.SheetName} : ITableData<_{sheet.SheetName}, {mCurrentHeader.KeyTypeName}>");
            sw.WriteLine("\t{");
            sw.WriteLine($"\t\tpublic {mCurrentHeader.KeyTypeName} GetKey() {{ return {mCurrentHeader.KeyFieldName}; }}");
            int index = 0;
            for (int c = 0; c < mCurrentHeader.MaxColumn; c++)
            {
                var data = mCurrentHeader.Get(c);
                if (data == null)
                    continue;
                sw.WriteLine($"\t\t[Key({index++})]");
                if (data.isList)
                    sw.WriteLine(string.Format("\t\tpublic {0,-20} {1};", data.fieldType + "[]", data.fieldName));
                else
                    sw.WriteLine(string.Format("\t\tpublic {0,-20} {1};", data.fieldType, data.fieldName));
            }
            sw.WriteLine("");
            sw.WriteLine($"\t\tpublic void Initialize(_{sheet.SheetName} data)");
            sw.WriteLine("\t\t{");
            for (int c = 0; c < mCurrentHeader.MaxColumn; c++)
            {
                var data = mCurrentHeader.Get(c);
                if (data == null)
                    continue;
                sw.WriteLine($"\t\t\t{data.fieldName} = data.get_{data.fieldName}();");
            }
            sw.WriteLine("\t\t}");
            sw.WriteLine("\t}");
            sw.WriteLine("");
        }
    }
}
