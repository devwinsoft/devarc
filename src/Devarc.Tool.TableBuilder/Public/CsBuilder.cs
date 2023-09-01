using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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
                    onReadSheet(sheet, sw);
                }
                sw.WriteLine("}");
                sw.Close();
            }
            Console.WriteLine($"Generate file: {filePath}");
        }


        void onReadSheet(ISheet sheet, StreamWriter sw)
        {
            sw.WriteLine("\t[MessagePackObject]");
            sw.WriteLine($"\tpublic class {sheet.SheetName}");
            sw.WriteLine("\t{");
            int index = 0;
            for (int c = 0; c < mHeaderInfo.MaxColumn; c++)
            {
                var data = mHeaderInfo.Get(c);
                if (data == null)
                    continue;
                sw.WriteLine($"\t\t[Key({index++})]");
                if (data.isList)
                    sw.WriteLine(string.Format("\t\tpublic {0,-20} {1};", data.fieldType + "[]", data.fieldName));
                else
                    sw.WriteLine(string.Format("\t\tpublic {0,-20} {1};", data.fieldType, data.fieldName));
            }
            sw.WriteLine("\t}");
        }
    }
}
