using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NPOI;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF;
using NPOI.XSSF.UserModel;

namespace Devarc
{
    public class JsonBuilder : BaseBuilder
    {
        public void Build(string inputPath)
        {
            var doc = open(inputPath);
            for (int i = 0; i < doc.NumberOfSheets; i++)
            {
                var sheet = doc.GetSheetAt(i);
                readHeader(sheet);
                onReadSheet(sheet);
            }
        }

        void onReadSheet(ISheet sheet)
        {
            var workingDir = Path.GetFullPath(".\\");
            var filePath = Path.Combine(workingDir, $"{sheet.SheetName}.json");

            using (var sw = File.CreateText(filePath))
            {
                sw.Write("{\"list\":[");
                bool isFirstLine = true;
                for (int r = (int)RowType.Data; r <= sheet.LastRowNum; r++)
                {
                    var row = sheet.GetRow(r);
                    onReadRow(row, sw, isFirstLine);
                    isFirstLine = false;
                }
                sw.WriteLine("]}");
                sw.Close();
            }
            Console.WriteLine($"Generate file: {filePath}");
        }


        void onReadRow(IRow row, StreamWriter sw, bool isFirstLine)
        {
            var cells = row.Cells;
            if (isFirstLine)
                sw.Write("{");
            else
                sw.Write(",{ ");
            bool started = false;
            for (int c = 0; c < cells.Count; c++)
            {
                var header = mCurrentHeader.Get(c);
                if (header == null)
                    continue;
                var value = cells[c].ToString();
                if (started == false)
                    started = true;
                else
                    sw.Write($",");

                var typeName = header.fieldName.ToLower();
                if (needQuotes(value))
                    sw.Write($"\"{header.fieldName}\":\"{value}\"");
                else
                    sw.Write($"\"{header.fieldName}\":{value}");
            }
            sw.WriteLine("}");
        }


        bool needQuotes(string value)
        {
            float temp;
            return !float.TryParse(value, out temp);
        }
    }
}
