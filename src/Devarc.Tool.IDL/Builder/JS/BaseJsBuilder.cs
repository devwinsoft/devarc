using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Devarc
{
    internal abstract class BaseJsBuilder : BaseBuilder
    {
        protected void generate_js_class(ClassFileData data)
        {
            string dllPath = Path.Combine(mAssemDir, $"{data.fileName}.dll");
            string outputPath = $"{mWorkingDir}\\{data.fileName}.js";
            StringBuilder output = new StringBuilder();

            foreach (Type tp in data.types)
            {
                generate_js_pass1(tp, output);
            }

            int line = 0;
            output.AppendLine("module.exports =");
            foreach (Type tp in data.types)
            {
                if (line == 0)
                    output.AppendLine($"{{ {tp.Name}");
                else
                    output.AppendLine($", {tp.Name}");
                line++;
            }
            output.AppendLine("}");
            output.AppendLine("");

            File.WriteAllText(outputPath, output.ToString());
        }


        protected void generate_js_pass1(Type _type, StringBuilder _output)
        {
            if (IsClass(_type))
            {
                var fields = _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                _output.AppendLine($"class {_type.Name}");
                _output.AppendLine("{");
                _output.AppendLine("\t/**");
                foreach (FieldInfo finfo in fields)
                {
                    var typeName = ToTypeName(finfo);
                    _output.AppendLine($"\t * @param {{{typeName}}} {finfo.Name} - {typeName}");
                }
                _output.AppendLine("\t */");
                _output.AppendLine("\tconstructor() {");
                foreach (FieldInfo finfo in _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (IsArray(finfo))
                    {
                        _output.AppendLine($"\t\tthis.{finfo.Name} = [];");
                    }
                    else if (IsClass(finfo))
                    {
                        _output.AppendLine($"\t\t\tthis.{finfo.Name} = new {finfo.FieldType.Name}();");
                    }
                    else
                    {
                        if (finfo.FieldType == typeof(string))
                        {
                            _output.AppendLine($"\t\tthis.{finfo.Name} = \"\";");
                        }
                        else
                        {
                            _output.AppendLine($"\t\tthis.{finfo.Name} = 0;");
                        }
                    }
                }
                _output.AppendLine("\t}");
                _output.AppendLine("\tInit(packet) {");
                int fcount = 0;
                foreach (FieldInfo finfo in _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (IsArray(finfo))
                    {
                        if (IsClass(finfo.FieldType.GetElementType()))
                        {
                            var rawName = finfo.FieldType.Name;
                            var pureName = rawName.Substring(0, rawName.Length - 2);
                            _output.AppendLine($"\t\tthis.{finfo.Name} = [];");
                            _output.AppendLine($"\t\tfor (let i = 0; i < packet[{fcount}].length; i++)");
                            _output.AppendLine($"\t\t{{");
                            _output.AppendLine($"\t\t\tvar obj = new {pureName}();");
                            _output.AppendLine($"\t\t\tobj.Init(packet[{fcount}][i]);");
                            _output.AppendLine($"\t\t\tthis.{finfo.Name}.push(obj);");
                            _output.AppendLine($"\t\t}}");
                        }
                        else
                        {
                            _output.AppendLine($"\t\tthis.{finfo.Name} = packet[{fcount}];");
                        }
                    }
                    else if (IsClass(finfo))
                    {
                        _output.AppendLine($"\t\tthis.{finfo.Name} = new {finfo.FieldType.Name}();");
                        _output.AppendLine($"\t\tthis.{finfo.Name}.Init(packet[{fcount}]);");
                    }
                    else
                    {
                        _output.AppendLine($"\t\tthis.{finfo.Name} = packet[{fcount}];");
                    }
                    fcount++;
                }
                _output.AppendLine("\t}");
                _output.AppendLine("\tToArray() {");
                _output.AppendLine("\t\tconst data =");
                _output.AppendLine("\t\t[");
                foreach (FieldInfo finfo in _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (IsArray(finfo))
                    {
                        if (IsClass(finfo.FieldType.GetElementType()))
                        {
                            _output.AppendLine($"\t\t\ttoArray(this.{finfo.Name}),");
                        }
                        else
                        {
                            _output.AppendLine($"\t\t\tthis.{finfo.Name},");
                        }
                    }
                    else if (IsClass(finfo))
                    {
                        _output.AppendLine($"\t\t\tthis.{finfo.Name}.ToArray(),");
                    }
                    else
                    {
                        _output.AppendLine($"\t\t\tthis.{finfo.Name},");
                    }
                }
                _output.AppendLine("\t\t];");
                _output.AppendLine("\t\treturn data;");
                _output.AppendLine("\t}");
                _output.AppendLine("}");
                _output.AppendLine("");
            }
            else if (IsEnum(_type))
            {
                Type enumUnderlyingType = System.Enum.GetUnderlyingType(_type);
                Array enumValues = System.Enum.GetValues(_type);

                _output.AppendLine($"const {_type.Name} = {{");
                for (int i = 0; i < enumValues.Length; i++)
                {
                    object value = enumValues.GetValue(i);
                    object underlyingValue = Convert.ChangeType(value, enumUnderlyingType);
                    _output.AppendLine(string.Format("\t{0,-20} : {1},", value, underlyingValue));
                }
                _output.AppendLine("};");
                _output.AppendLine($"Object.freeze({_type.Name});");
                _output.AppendLine("");
            }
        }

    }
}
