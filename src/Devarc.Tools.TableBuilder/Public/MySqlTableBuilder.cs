﻿using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Devarc
{
    public class MySqlTableBuilder : BaseTableBuilder
    {
        enum VAR_TYPE
        {
            BOOL,
            FLOAT,
            INT,
            STRING,
        }


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
            var ddlPath = $"{sheet.SheetName}.ddl";
            using (var sw = File.CreateText(ddlPath))
            {
                //
                sw.WriteLine($"DROP TABLE IF EXISTS `{mCurrentHeader.SheetName}`;");
                sw.WriteLine($"CREATE TABLE `{mCurrentHeader.SheetName}` (");
                for (int i = 0;i<mCurrentHeader.MaxColumn;i++)
                {
                    var header = mCurrentHeader.Get(i);
                    if (header == null)
                        continue;

                    switch (GetType(header.fieldType))
                    {
                        case VAR_TYPE.BOOL:
                            sw.WriteLine($"\t`{header.fieldName}` BOOLEAN NOT NULL,");
                            break;
                        case VAR_TYPE.FLOAT:
                            sw.WriteLine($"\t`{header.fieldName}` FLOAT NOT NULL,");
                            break;
                        case VAR_TYPE.INT:
                            sw.WriteLine($"\t`{header.fieldName}` INT NOT NULL,");
                            break;
                        default:
                            if (mCurrentHeader.KeyFieldName == header.fieldName)
                                sw.WriteLine($"\t`{header.fieldName}` varchar(50) NOT NULL,");
                            else
                                sw.WriteLine($"\t`{header.fieldName}` varchar(255) NOT NULL,");
                            break;
                    }
                }
                sw.WriteLine($"\tPRIMARY KEY (`{mCurrentHeader.KeyFieldName}`)");
                sw.WriteLine(") ENGINE=MyISAM DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci");
                sw.Close();
            }


            var sqlPath = $"{sheet.SheetName}.sql";
            using (var sw = File.CreateText(sqlPath))
            {
                sw.WriteLine($"truncate `{sheet.SheetName}`;");
                for (int r = (int)RowType.Data; r <= sheet.LastRowNum; r++)
                {
                    var started = false;
                    var row = sheet.GetRow(r);
                    if (row == null)
                        continue;

                    sw.Write($"insert into `{sheet.SheetName}` (");
                    for (int c = 0; c < row.LastCellNum; c++)
                    {
                        var header = mCurrentHeader.Get(c);
                        if (header == null)
                            continue;
                        if (started)
                            sw.Write(", ");
                        sw.Write($"`{header.fieldName}`");
                        started = true;
                    }
                    sw.Write(") values (");
                    started = false;
                    for (int c = 0; c < row.LastCellNum; c++)
                    {
                        var header = mCurrentHeader.Get(c);
                        if (header == null)
                            continue;

                        var cell = row.GetCell(c);
                        if (cell == null)
                            continue;

                        if (started)
                            sw.Write(", ");

                        switch (GetType(header.fieldType))
                        {
                            case VAR_TYPE.BOOL:
                                {
                                    bool value;
                                    bool.TryParse(cell.ToString(), out value);
                                    sw.Write(value);
                                }
                                break;
                            case VAR_TYPE.FLOAT:
                                {
                                    float value;
                                    float.TryParse(cell.ToString(), out value);
                                    sw.Write(value);
                                }
                                break;
                            case VAR_TYPE.INT:
                                {
                                    int value;
                                    int.TryParse(cell.ToString(), out value);
                                    sw.Write(value);
                                }
                                break;
                            default:
                                sw.Write($"'{cell}'");
                                break;
                        }
                        started = true;
                    }
                    sw.WriteLine(");");
                }
                sw.Close();
            }
        }

        VAR_TYPE GetType(string typeName)
        {
            string temp = typeName.ToLower();
            switch (temp)
            {
                case "bool":
                case "boolean":
                    return VAR_TYPE.BOOL;
                case "int":
                case "short":
                    return VAR_TYPE.INT;
                case "float":
                case "double":
                    return VAR_TYPE.FLOAT;
                default:
                    return VAR_TYPE.STRING;
            }
        }
    }
}
