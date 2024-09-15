using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Devarc
{
    internal abstract class BaseCsBuilder : BaseBuilder
    {
        protected bool isParentField(Type parentType, FieldInfo field)
        {
            foreach (FieldInfo finfo in parentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (finfo.Name == field.Name)
                    return true;
            }
            return false;
        }

        protected void generate_cs_protocol(Type _type, StringBuilder _output)
        {
            if (IsClass(_type))
            {
                _output.AppendLine($"\t[MessagePackObject]");
                _output.AppendLine($"\tpublic partial class {_type.Name} : BaseTableElement<{_type.Name}>, ITableElement<{_type.Name}>");
                _output.AppendLine("\t{");
                int fcount = 0;
                foreach (FieldInfo finfo in _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    var typeName = ToTypeName(finfo);
                    _output.AppendLine($"\t\t[Key({fcount})]");
                    if (IsArray(finfo))
                        _output.AppendLine(string.Format("\t\tpublic {0,-20} {1} = new {0}();", typeName, finfo.Name));
                    else if (IsClass(finfo))
                        _output.AppendLine(string.Format("\t\tpublic {0,-20} {1} = new {0}();", typeName, finfo.Name));
                    else
                        _output.AppendLine(string.Format("\t\tpublic {0,-20} {1};", typeName, finfo.Name));
                    fcount++;
                }
                _output.AppendLine("\t}");
                _output.AppendLine("");
            }
            else if (IsEnum(_type))
            {
                Type enumUnderlyingType = System.Enum.GetUnderlyingType(_type);
                Array enumValues = System.Enum.GetValues(_type);

                if (_type.GetCustomAttributes<FlagsAttribute>().Any())
                    _output.AppendLine("\t[System.Flags]");
                _output.AppendLine($"\tpublic enum {_type.Name}");
                _output.AppendLine("\t{");
                for (int i = 0; i < enumValues.Length; i++)
                {
                    object value = enumValues.GetValue(i);
                    object underlyingValue = Convert.ChangeType(value, enumUnderlyingType);

                    _output.AppendLine(string.Format("\t\t{0,-20} = {1},", value, underlyingValue));
                }
                _output.AppendLine("\t}");
                _output.AppendLine("");
            }
        }

    }
}
