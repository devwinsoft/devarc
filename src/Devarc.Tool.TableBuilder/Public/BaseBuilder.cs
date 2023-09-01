using NPOI.HSSF.UserModel;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devarc
{
    public abstract class BaseBuilder
    {
        protected HeaderData mHeaderInfo = new HeaderData();

        protected IWorkbook open(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                switch (Path.GetExtension(filePath).ToLower())
                {
                    case "xls":
                        return new HSSFWorkbook(fs);
                    case "xlsx":
                    default:
                        return new XSSFWorkbook(fs);
                }
            }
        }

        protected void readHeader(ISheet sheet)
        {
            for (int r = 0; r < (int)RowType.Data; r++)
            {
                var row = sheet.GetRow(r);
                mHeaderInfo.Set(row, (RowType)r);
            }
        }
    }
}
