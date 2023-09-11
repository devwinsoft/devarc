using Microsoft.CSharp;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Devarc
{
    internal class JsBuilder : BaseJsBuilder
    {
        public void Build(string protocolFile, params string[] classFiles)
        {
            mAssemDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            mBakCurDir = Directory.GetCurrentDirectory();
            mWorkingDir = Path.GetFullPath(".\\");
            Directory.SetCurrentDirectory(mWorkingDir);

            if (classFiles.Length > 0)
            {
                compileClassFileData(classFiles[0]);
            }

            // Exeption handling
            if (File.Exists(protocolFile) == false)
            {
                Console.WriteLine($"Cannot find base file: {protocolFile}");
                Directory.SetCurrentDirectory(mBakCurDir);
                return;
            }

            // Generate source code.
            string srcData = File.ReadAllText(protocolFile);
            var protocolCP = new System.CodeDom.Compiler.CompilerParameters();
            protocolCP.GenerateExecutable = false;
            protocolCP.GenerateInMemory = true;
            if (classFiles.Length > 0)
            {
                string dllName = Path.GetFileNameWithoutExtension(classFiles[0]);
                string dllPath = Path.Combine(mAssemDir, $"{dllName}.dll");
                protocolCP.ReferencedAssemblies.Add(dllPath);
            }

            CSharpCodeProvider provider = new CSharpCodeProvider();
            var res = provider.CompileAssemblyFromSource(protocolCP, srcData);
            Assembly assem = null;
            try
            {
                assem = res.CompiledAssembly;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Directory.SetCurrentDirectory(mBakCurDir);
                return;
            }

            foreach (Type tp in assem.GetTypes())
            {
                registerTypeToNamespace(tp);
            }

            foreach (var temp in mNamespaceDatas)
            {
                StringBuilder output = new StringBuilder();
                string fileName = temp.Key;
                string outputPath = $"{mWorkingDir}\\{fileName}.js";

                output.AppendLine("const msgpack = require('msgpack-lite');");
                foreach (var data in mClassDatas)
                {
                    output.AppendLine($"const {data.fileName} = require('./{data.fileName}.js');");
                    foreach (var tp in data.types)
                    {
                        output.AppendLine($"const {tp.Name} = {data.fileName}.{tp.Name};");
                    }
                }
                output.AppendLine("const mHandlers = {};");

                foreach (Type tp in temp.Value)
                {
                    generate_js_pass1(tp, output);
                }

                int line = 0;
                output.AppendLine("module.exports =");
                foreach (Type tp in temp.Value)
                {
                    if (line == 0)
                        output.AppendLine($"{{ {tp.Name}");
                    else
                        output.AppendLine($", {tp.Name}");
                    line++;
                }
                output.AppendLine("}");
                output.AppendLine("");

                generate_js_pass2(temp.Value.ToArray(), output);
                File.WriteAllText(outputPath, output.ToString());
            }

            Directory.SetCurrentDirectory(mBakCurDir);
        }



        void generate_js_pass2(Type[] _types, StringBuilder _output)
        {
            _output.AppendLine("function createPacket(packetName, content)");
            _output.AppendLine("{");
            _output.AppendLine("\tswitch (packetName)");
            _output.AppendLine("\t{");
            foreach (Type tp in _types)
            {
                _output.AppendLine($"\tcase '{tp.Name}':");
                _output.AppendLine("\t\t{");
                _output.AppendLine($"\t\t\tconst obj = new {tp.Name}();");
                _output.AppendLine("\t\t\tobj.Init(content);");
                _output.AppendLine("\t\t\treturn obj;");
                _output.AppendLine("\t\t}");
            }
            _output.AppendLine("\t\tdefault:");
            _output.AppendLine("\t\t\treturn null;");
            _output.AppendLine("\t}");
            _output.AppendLine("}");
            _output.Append(sourceCode);
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
