using NPOI.POIFS.Crypt.Dsig;
using NPOI.POIFS.Storage;
using NPOI.SS.UserModel;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Devarc
{
    internal class LStrBuilder : BaseBuilder
    {
        class Data
        {
            public int index;
            public string language;
            public Dictionary<string, string> list;
        }
        Dictionary<int, Data> mDatas = new Dictionary<int, Data>();
        string mOutputDir;

        public void Build(string inputPath)
        {
            mOutputDir = Path.GetDirectoryName(inputPath);

            var doc = open(inputPath);
            for (int i = 0; i < doc.NumberOfSheets; i++)
            {
                var sheet = doc.GetSheetAt(i);
                generate(sheet);
            }
        }


        void generate(ISheet sheet)
        {
            // read header
            {
                var cells = sheet.GetRow(0).Cells;
                for (int c = 1; c < cells.Count; c++)
                {
                    Data data = new Data();
                    data.index = c;
                    data.language = cells[c].ToString();
                    data.list = new Dictionary<string, string>();
                    mDatas.Add(c, data);
                }
            }

            int maxColumnNum = -1;
            for (int r = 1; r <= sheet.LastRowNum; r++)
            {
                var cells = sheet.GetRow(r).Cells;
                var key = cells[0].ToString();
                for (int c = 1; c < cells.Count; c++)
                {
                    var value = cells[c].ToString();
                    Data data;
                    if (mDatas.TryGetValue(c, out data) == false)
                        continue;

                    data.list.Add(key, value);
                    maxColumnNum = Math.Max(c, maxColumnNum);
                }
            }

            for (int c = 1; c <= maxColumnNum; c++)
            {
                Data data;
                if (mDatas.TryGetValue(c, out data) == false)
                    continue;

                var code = data.language;
                var saveDir = Path.Combine(mOutputDir, code);
                if (Directory.Exists(saveDir) == false)
                {
                    Directory.CreateDirectory(saveDir);
                }

                var filePath = Path.Combine(saveDir, $"{sheet.SheetName}.json");
                bool isFirstLine = true;

                using (var sw = File.CreateText(filePath))
                {
                    Console.WriteLine($"Generate file: {filePath}");

                    sw.WriteLine("{\"list\":[");
                    foreach (var obj in data.list)
                    {
                        if (isFirstLine)
                        {
                            isFirstLine = false;
                            sw.WriteLine($" {{\"id\":\"{obj.Key}\", \"value\":\"{obj.Value}\"}}");
                        }
                        else
                        {
                            sw.WriteLine($",{{\"id\":\"{obj.Key}\", \"value\":\"{obj.Value}\"}}");
                        }
                    }
                    sw.WriteLine("]}");
                    sw.Close();
                }
            }
        }


        string ToISO639_2(string language)
        {
            switch (language.ToLower())
            {
                case "korean": return "kor";
                case "japanese": return "jpn";
                case "chinease": return "zho";
                default: return "eng";
            }
        }
    }

}
