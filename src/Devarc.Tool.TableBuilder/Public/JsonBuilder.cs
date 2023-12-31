﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NPOI.SS.UserModel;
using System.Text.RegularExpressions;

namespace Devarc
{
    public class JsonBuilder : BaseBuilder
    {
        Regex mRegex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");

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
                sw.WriteLine("{\"list\":[");
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
            if (isFirstLine)
                sw.Write("{");
            else
                sw.Write(",{ ");
            bool started = false;
            for (int c = 0; c < row.LastCellNum; c++)
            {
                var header = mCurrentHeader.Get(c);
                if (header == null)
                    continue;
                var cell = row.GetCell(c);
                if (cell == null)
                    continue;
                var value = cell.ToString();
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
            return !mRegex.IsMatch(value);
        }
    }
}
