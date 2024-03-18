using NPOI.OpenXmlFormats;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devarc
{
    public enum RowType
    {
        Description = 0,
        FieldName,
        FieldType,
        ClassType,
        Data,
    }

    public class FieldData
    {
        public string description;
        public string fieldName;
        public string fieldType;
        public bool isClass;
        public bool isList;
    }

    public class HeaderData
    {
        public string SheetName = string.Empty;
        public string KeyTypeName = string.Empty;
        public string KeyFieldName = string.Empty;
        public bool ShowKey = false;
        public string DisplayName = string.Empty;
        public bool IsDataSheet = false;
        public int MaxColumn => mMaxColumn;
        int mMaxColumn = 0;

        Dictionary<int, FieldData> fields = new Dictionary<int, FieldData>();

        public FieldData Get(int column)
        {
            FieldData value = null;
            fields.TryGetValue(column, out value);
            return value;
        }

        public void Set(IRow row, RowType rowType)
        {
            mMaxColumn = Math.Max(mMaxColumn, row.LastCellNum);

            for (int c = 0; c < row.LastCellNum; c++) 
            {
                var cell = row.GetCell(c);

                FieldData field = null;
                if (rowType == RowType.Description)
                {
                    field = new FieldData();
                    fields[c] = field;
                }
                else
                {
                    field = fields[c];
                }

                if (cell == null)
                    continue;

                switch (rowType)
                {
                    case RowType.Description:
                        field.description = cell.ToString().Trim();
                        break;
                    case RowType.FieldName:
                        field.fieldName = cell.ToString().Trim();
                        break;
                    case RowType.FieldType:
                        var typeName = cell.ToString().Trim();
                        if (typeName.EndsWith("[]"))
                        {
                            field.fieldType = typeName.Substring(0, typeName.Length - 2);
                            field.isList = true;
                        }
                        else
                        {
                            field.fieldType = cell.ToString().Trim();
                        }
                        break;
                    case RowType.ClassType:
                        string options = cell.ToString().ToLower();
                        field.isClass = options.Contains("class");
                        if (options.Contains("key"))
                        {
                            KeyTypeName = field.fieldType;
                            KeyFieldName = field.fieldName;
                            if (options.Contains("show"))
                                ShowKey = true;
                        }
                        else if (options.Contains("display"))
                        {
                            DisplayName = field.fieldName;
                        }
                        break;
                }
            }
        }
    }
}
