using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devarc
{
    internal enum RowType
    {
        Description = 0,
        FieldName,
        FieldType,
        ClassType,
        Data,
    }

    internal class FieldData
    {
        public string description;
        public string fieldName;
        public string fieldType;
        public bool isClass;
        public bool isList;
    }

    internal class HeaderData
    {
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
            var cells = row.Cells;

            mMaxColumn = Math.Max(mMaxColumn, cells.Count);

            for (int col = 0; col < cells.Count; col++) 
            {
                var cell = row.GetCell(col);
                if (cell == null)
                    continue;

                FieldData field = null;
                if (rowType == RowType.Description)
                {
                    field = new FieldData();
                    fields[col] = field;
                }
                else
                {
                    field = fields[col];
                }

                switch (rowType)
                {
                    case RowType.Description:
                        field.description = cell.ToString().Trim();
                        break;
                    case RowType.FieldName:
                        field.fieldName = cell.ToString().Trim();
                        break;
                    case RowType.FieldType:
                        field.fieldType = cell.ToString().Trim();
                        break;
                    case RowType.ClassType:
                        string types = cell.ToString().ToLower();
                        field.isClass = types.Contains("class");
                        field.isList = types.Contains("list");
                        break;
                }
            }
        }
    }
}
