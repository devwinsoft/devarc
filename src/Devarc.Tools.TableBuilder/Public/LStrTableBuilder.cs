using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devarc
{
    internal class LStrTableBuilder : BaseTableBuilder
    {
        class Data
        {
            public int index;
            public string language;
            public Dictionary<string, string> list;
        }

        public void Build(SettingConfig cfg, string inputPath)
        {
            var doc = open(inputPath);
            for (int i = 0; i < doc.NumberOfSheets; i++)
            {
                var sheet = doc.GetSheetAt(i);
                generate(cfg, sheet);
            }
        }


        void generate(SettingConfig cfg, ISheet sheet)
        {
            Dictionary<int, Data> sheetDatas = new Dictionary<int, Data>();

            // read header
            {
                var row = sheet.GetRow(0);
                for (int c = 1; c < row.LastCellNum; c++)
                {
                    var cell = row.GetCell(c);
                    if (cell == null)
                        continue;
                    Data data = new Data();
                    data.index = c;
                    data.language = cell.ToString();
                    data.list = new Dictionary<string, string>();
                    sheetDatas.Add(c, data);
                }
            }

            int maxColumnNum = -1;
            for (int r = 1; r <= sheet.LastRowNum; r++)
            {
                var row = sheet.GetRow(r);
                if (row == null)
                    continue;

                var key = row.GetCell(0).ToString();
                for (int c = 1; c < row.LastCellNum; c++)
                {
                    var cell = row.GetCell(c);
                    var value = cell.ToString();
                    Data data;
                    if (sheetDatas.TryGetValue(c, out data) == false)
                        continue;

                    data.list.Add(key, value);
                    maxColumnNum = Math.Max(c, maxColumnNum);
                }
            }

            for (int c = 1; c <= maxColumnNum; c++)
            {
                Data data;
                if (sheetDatas.TryGetValue(c, out data) == false)
                    continue;
                
                var filePath = $"{sheet.SheetName}.json";
                bool isFirstLine = true;

                using (var sw = File.CreateText(filePath))
                {
                    Console.WriteLine($"Generate file: {data.language}/{filePath}");

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

                bool isResource = filePath.ToLower().Contains("resource");
                var folderName = isResource
                    ? Path.Combine(cfg.language_resource, data.language)
                    : Path.Combine(cfg.language_bundle, data.language);
                if (Directory.Exists(folderName) == false)
                    Directory.CreateDirectory(folderName);
                var destPath = Path.Combine(folderName, filePath);
                File.Copy(filePath, destPath, true);
                File.Delete(filePath);
                Console.WriteLine($"Copy to: {destPath}");
            }
        }

    }

}
