using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Devarc
{
    internal abstract class BaseJsBuilder : BaseBuilder
    {
        protected void generate_js_code(SourceData data)
        {
            string fileName = $"{data.fileName}.js";
            StringBuilder output = new StringBuilder();

            foreach (var symbol in data.symbols)
            {
                generate_js_class(symbol, output);
            }

            int line = 0;
            output.AppendLine("module.exports =");
            foreach (var symbol in data.symbols)
            {
                if (line == 0)
                    output.AppendLine($"{{ {symbol.Name}");
                else
                    output.AppendLine($", {symbol.Name}");
                line++;
            }
            output.AppendLine("}");
            output.AppendLine("");

            File.WriteAllText(fileName, output.ToString());
        }


        protected void generate_js_class(ITypeSymbol symbol, StringBuilder output)
        {
            if (symbol.TypeKind == TypeKind.Class)
            {
                output.AppendLine($"class {symbol.Name}");
                output.AppendLine("{");
                output.AppendLine("\t/**");
                foreach (var member in symbol.GetMembers())
                {
                    var typeName = ToTypeName(member);
                    if (string.IsNullOrEmpty(typeName))
                        continue;
                    output.AppendLine($"\t * @param {{{typeName}}} {member.Name} - {typeName}");
                }
                output.AppendLine("\t */");
                output.AppendLine("\tconstructor() {");
                foreach (var member in symbol.GetMembers())
                {
                    var typeName = ToTypeName(member);
                    if (string.IsNullOrEmpty(typeName))
                        continue;
                    if (member is IArrayTypeSymbol arraySymbol)
                    {
                        output.AppendLine($"\t\tthis.{member.Name} = [];");
                    }
                    else if (member is INamedTypeSymbol namedSymbol && namedSymbol.TypeKind == TypeKind.Class)
                    {
                        output.AppendLine($"\t\t\tthis.{member.Name} = new {member.Name}();");
                    }
                    else
                    {
                        if (member.Name.ToLower() == "string")
                        {
                            output.AppendLine($"\t\tthis.{member.Name} = \"\";");
                        }
                        else
                        {
                            output.AppendLine($"\t\tthis.{member.Name} = 0;");
                        }
                    }
                }
                output.AppendLine("\t}");
                output.AppendLine("\tInit(packet) {");
                int fcount = 0;
                foreach (var member in symbol.GetMembers())
                {
                    var typeName = ToTypeName(member);
                    if (string.IsNullOrEmpty(typeName))
                        continue;
                    if (member is IArrayTypeSymbol arrayTypeSymbol)
                    {
                        if (arrayTypeSymbol.TypeKind == TypeKind.Class)
                        {
                            var rawName = member.Name;
                            var pureName = rawName.Substring(0, rawName.Length - 2);
                            output.AppendLine($"\t\tthis.{member.Name} = [];");
                            output.AppendLine($"\t\tfor (let i = 0; i < packet[{fcount}].length; i++)");
                            output.AppendLine($"\t\t{{");
                            output.AppendLine($"\t\t\tvar obj = new {pureName}();");
                            output.AppendLine($"\t\t\tobj.Init(packet[{fcount}][i]);");
                            output.AppendLine($"\t\t\tthis.{member.Name}.push(obj);");
                            output.AppendLine($"\t\t}}");
                        }
                        else
                        {
                            output.AppendLine($"\t\tthis.{member.Name} = packet[{fcount}];");
                        }
                    }
                    else if (member is INamedTypeSymbol namedSymbol && namedSymbol.TypeKind == TypeKind.Class)
                    {
                        output.AppendLine($"\t\tthis.{member.Name} = new {member.Name}();");
                        output.AppendLine($"\t\tthis.{member.Name}.Init(packet[{fcount}]);");
                    }
                    else
                    {
                        output.AppendLine($"\t\tthis.{member.Name} = packet[{fcount}];");
                    }
                    fcount++;
                }
                output.AppendLine("\t}");
                output.AppendLine("\tToArray() {");
                output.AppendLine("\t\tconst data =");
                output.AppendLine("\t\t[");
                foreach (var member in symbol.GetMembers())
                {
                    var typeName = ToTypeName(member);
                    if (string.IsNullOrEmpty(typeName))
                        continue;
                    if (member is IArrayTypeSymbol arrayMember)
                    {
                        if (arrayMember.TypeKind == TypeKind.Class)
                        {
                            output.AppendLine($"\t\t\ttoArray(this.{member.Name}),");
                        }
                        else
                        {
                            output.AppendLine($"\t\t\tthis.{member.Name}.ToArray(),");
                        }
                    }
                    else if (member is INamedTypeSymbol)
                    {
                        output.AppendLine($"\t\t\tthis.{member.Name},");
                    }
                    else
                    {
                        output.AppendLine($"\t\t\tthis.{member.Name},");
                    }
                }
                output.AppendLine("\t\t];");
                output.AppendLine("\t\treturn data;");
                output.AppendLine("\t}");
                output.AppendLine("}");
                output.AppendLine("");
            }
            else if (symbol.TypeKind == TypeKind.Enum)
            {
                bool hasFlags = symbol.GetAttributes().Any(attr =>
                    attr.AttributeClass?.ToDisplayString() == "System.FlagsAttribute");
                output.AppendLine($"const {symbol.Name} = {{");
                foreach (var member in symbol.GetMembers().OfType<IFieldSymbol>())
                {
                    if (member.ConstantValue != null)
                    {
                        output.AppendLine(string.Format("\t{0,-20} = {1},", member.Name, member.ConstantValue));
                    }
                }
                output.AppendLine("};");
                output.AppendLine($"Object.freeze({symbol.Name});");
                output.AppendLine("");
            }
        }
        
        
        protected void generate_js_packet(List<INamedTypeSymbol> symbols, StringBuilder output)
        {
            output.AppendLine("function createPacket(packetName, content)");
            output.AppendLine("{");
            output.AppendLine("\tswitch (packetName)");
            output.AppendLine("\t{");
            foreach (var symbol in symbols)
            {
                output.AppendLine($"\tcase '{symbol.Name}':");
                output.AppendLine("\t\t{");
                output.AppendLine($"\t\t\tconst obj = new {symbol.Name}();");
                output.AppendLine("\t\t\tobj.Init(content);");
                output.AppendLine("\t\t\treturn obj;");
                output.AppendLine("\t\t}");
            }
            output.AppendLine("\t\tdefault:");
            output.AppendLine("\t\t\treturn null;");
            output.AppendLine("\t}");
            output.AppendLine("}");
            output.Append(sourceCode);
        }

        
        
        string sourceCode = $@"
function toArray(list)
{{
	var result = [];
	for (let i = 0; i < list.length; i++)
	{{
		result.push(list[i].ToArray());
	}}
	return result;
}}
function unpack(buf)
{{
    var len = buf.length;
    var type_len = new Uint32Array(buf.slice(0, 2))[0];
    var type_name = buf.slice(2, 2 + type_len).toString();
    var data = buf.slice(2 + type_len);
    var content = msgpack.decode(data);

    return createPacket(type_name, content);
}}

module.exports.pack = (obj) =>
{{
    var type_name = obj.constructor.name;
    var buf_length = Buffer.alloc(2);
    var buf_name = Buffer.alloc(type_name.length);
    var buf_data = msgpack.encode(obj.ToArray());

    buf_length.writeUInt16LE(type_name.length, 0);
    //buf_length.writeUInt16BE(type_name.length, 0);
    buf_name.write(type_name);

    var arr = new Buffer.alloc(buf_length.length + buf_name.length + buf_data.length);
    var offset = 0;
    arr.set(buf_length, offset); offset += buf_length.length;
    arr.set(buf_name, offset); offset += buf_name.length;
    arr.set(buf_data, offset);
    return arr;
}}

module.exports.on = (packetName, callback) =>
{{
	mHandlers[packetName] = callback;
}}

module.exports.dispatch = (packet, p1, p2) =>
{{
	var obj = unpack(packet);
    if (obj == null) return;
	var type_name = obj.constructor.name;
	var handler = mHandlers[type_name];
	if (handler != undefined)
	{{
		handler(obj, p1, p2);
	}}
}}
";
    }
}
