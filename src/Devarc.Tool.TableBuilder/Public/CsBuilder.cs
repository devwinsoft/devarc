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


        void onReadSheet(ISheet sheet, StreamWriter sw)
        {
            sw.WriteLine("\t[System.Serializable]");
            sw.WriteLine("\t[MessagePackObject]");
            sw.WriteLine($"\tpublic class {sheet.SheetName} : ITableData<{mCurrentHeader.KeyTypeName}>");
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
            sw.WriteLine("\t}");
            sw.WriteLine("");
        }
    }
}
