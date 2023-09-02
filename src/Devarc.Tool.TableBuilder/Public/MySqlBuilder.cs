using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devarc
{
    public class MySqlBuilder : BaseBuilder
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
            var workingDir = Path.GetFullPath(".\\");
            var ddlPath = Path.Combine(workingDir, $"{sheet.SheetName}.ddl");
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


            var sqlPath = Path.Combine(workingDir, $"{sheet.SheetName}.sql");
            using (var sw = File.CreateText(sqlPath))
            {
                sw.WriteLine($"truncate `{sheet.SheetName}`;");
                for (int r = (int)RowType.Data; r <= sheet.LastRowNum; r++)
                {
                    var row = sheet.GetRow(r);
                    var cells = row.Cells;
                    var started = false;

                    sw.Write($"insert into `{sheet.SheetName}` (");
                    for (int c = 0; c < cells.Count; c++)
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
                    for (int c = 0; c < cells.Count; c++)
                    {
                        var header = mCurrentHeader.Get(c);
                        if (header == null)
                            continue;
                        if (started)
                            sw.Write(", ");

                        switch (GetType(header.fieldType))
                        {
                            case VAR_TYPE.BOOL:
                                {
                                    bool value;
                                    bool.TryParse(cells[c].ToString(), out value);
                                    sw.Write(value);
                                }
                                break;
                            case VAR_TYPE.FLOAT:
                                {
                                    float value;
                                    float.TryParse(cells[c].ToString(), out value);
                                    sw.Write(value);
                                }
                                break;
                            case VAR_TYPE.INT:
                                {
                                    int value;
                                    int.TryParse(cells[c].ToString(), out value);
                                    sw.Write(value);
                                }
                                break;
                            default:
                                sw.Write($"'{cells[c]}'");
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
