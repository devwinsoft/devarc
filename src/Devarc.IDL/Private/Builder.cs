//
// Copyright (c) 2023 Hyoung Joon, Kim
// https://github.com/devwinsoft
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using Microsoft.CSharp;


namespace Devarc
{
    internal class Builder
    {
        class ClassFileData
        {
            public string fileName;
            public string filePath;
            public Type[] types;
        }
        List<ClassFileData> mClassDatas = new List<ClassFileData>();
        Dictionary<string, List<Type>> mNamespaceDatas = new Dictionary<string, List<Type>>();

        string mAssemDir;
        string mWorkingDir;
        string mBakCurDir;


        public void Build_CS(string protocolFile, params string[] classFiles)
        {
            mAssemDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            mBakCurDir = Directory.GetCurrentDirectory();
            mWorkingDir = Path.GetFullPath(".\\");
            Directory.SetCurrentDirectory(mWorkingDir);

            // Exeption handling
            if (File.Exists(protocolFile) == false)
            {
                Console.WriteLine($"Cannot find base file: {protocolFile}");
                Directory.SetCurrentDirectory(mBakCurDir);
                return;
            }

            if (classFiles.Length > 0)
            {
                compileClassFileData(classFiles[0]);
            }
            foreach (var temp in mClassDatas)
            {
                generate_cs_class(temp);
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
                string outputPath = $"{mWorkingDir}\\{fileName}.cs";

                output.AppendLine("using System.Collections.Generic;");
                output.AppendLine("using MessagePack;");
                output.AppendLine("using Devarc;");
                output.AppendLine("");
                output.AppendLine($"namespace {fileName}");
                output.AppendLine("{");
                foreach (Type tp in temp.Value)
                {
                    generate_cs_protocol(tp, output);
                }
                output.AppendLine("};");

                File.WriteAllText(outputPath, output.ToString());
            }

            Directory.SetCurrentDirectory(mBakCurDir);
        }



        private bool compileClassFileData(string classFilePath)
        {
            string sourceCode = File.ReadAllText(classFilePath);
            string dllName = Path.GetFileNameWithoutExtension(classFilePath);
            string dllPath = Path.Combine(mAssemDir, $"{dllName}.dll");

            // Initialize CompilerParameters
            var cp = new System.CodeDom.Compiler.CompilerParameters();
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = false;
            cp.OutputAssembly = dllPath;

            // Generate DLL
            try
            {
                CSharpCodeProvider provider = new CSharpCodeProvider();
                var cr = provider.CompileAssemblyFromSource(cp, sourceCode);
                if  (cr.Errors.Count == 0)
                {
                    var data = new ClassFileData();
                    data.fileName = dllName;
                    data.filePath = dllPath;
                    data.types = cr.CompiledAssembly.GetTypes();
                    mClassDatas.Add(data);
                    return true;
                }
                else
                {
                    foreach (var err in cr.Errors)
                    {
                        Console.WriteLine(err.ToString());
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        void registerTypeToNamespace(Type type)
        {
            List<Type> list = null;
            if (mNamespaceDatas.TryGetValue(type.Namespace, out list) == false)
            {
                list = new List<Type>();
                mNamespaceDatas[type.Namespace] = list;
            }
            list.Add(type);
        }

        bool IsArray(FieldInfo finfo) => finfo.FieldType.IsArray;
        bool IsArray(Type type) => type.IsArray;
        bool IsClass(FieldInfo finfo) => IsClass(finfo.FieldType);
        bool IsClass(Type type) => type.IsClass && !type.Name.ToLower().Contains("string");
        bool IsEnum(FieldInfo field) => field.FieldType.IsEnum;
        bool IsEnum(Type type) => type.IsEnum;

        string ToTypeName(FieldInfo finfo)
        {
            try
            {
                string typeName = finfo.FieldType.Name;
                string typeNameEx = finfo.FieldType.Name.ToLower();
                if (typeNameEx.Contains("[]"))
                {
                    if (typeNameEx.Contains("uint64"))
                        return "List<ulong>";
                    if (typeNameEx.Contains("uint32"))
                        return "List<uint>";
                    if (typeNameEx.Contains("uint16"))
                        return "List<ushort>";
                    if (typeNameEx.Contains("int64"))
                        return "List<long>";
                    if (typeNameEx.Contains("int32"))
                        return "List<int>";
                    if (typeNameEx.Contains("int16"))
                        return "List<short>";
                    if (typeNameEx.Contains("float") || typeNameEx.Contains("single"))
                        return "List<float>";
                    if (typeNameEx.Contains("byte"))
                        return "List<byte>";
                    if (typeNameEx.Contains("string"))
                        return "List<string>";
                    return $"List<{typeName.Substring(0, typeName.Length - 2)}>"; ;
                }
                else
                {
                    if (typeNameEx.Contains("boolean"))
                        return "bool";
                    if (typeNameEx.Contains("uint64"))
                        return "ulong";
                    if (typeNameEx.Contains("uint32"))
                        return "uint";
                    if (typeNameEx.Contains("uint16"))
                        return "ushort";
                    if (typeNameEx.Contains("int64"))
                        return "long";
                    if (typeNameEx.Contains("int32"))
                        return "int";
                    if (typeNameEx.Contains("int16"))
                        return "short";
                    if (typeNameEx.Contains("float") || typeNameEx.Contains("single"))
                        return "float";
                    if (typeNameEx.Contains("byte"))
                        return "byte";
                    if (typeNameEx.Contains("string"))
                        return "string";
                    return finfo.FieldType.ToString();
                }
            }
            catch
            {
                return "Unknown";
            }
        }


        void generate_cs_class(ClassFileData data)
        {
            StringBuilder output = new StringBuilder();
            string outputPath = $"{mWorkingDir}\\{data.fileName}.cs";

            output.AppendLine("using System.Collections.Generic;");
            output.AppendLine("using MessagePack;");
            output.AppendLine("");
            output.AppendLine($"namespace Devarc");
            output.AppendLine("{");
            foreach (Type tp in data.types)
            {
                generate_cs_protocol(tp, output);
            }
            output.AppendLine("};");

            File.WriteAllText(outputPath, output.ToString());
        }


        void generate_cs_protocol(Type _type, StringBuilder _output)
        {
            if (IsClass(_type))
            {
                _output.AppendLine($"\t[MessagePackObject]");
                _output.AppendLine($"\tpublic class {_type.Name}");
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



        public void Build_JS(string protocolFile, params string[] classFiles)
        {
            mAssemDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            mBakCurDir = Directory.GetCurrentDirectory();
            mWorkingDir = Path.GetFullPath(".\\");
            Directory.SetCurrentDirectory(mWorkingDir);

            if (classFiles.Length > 0)
            {
                compileClassFileData(classFiles[0]);
            }
            foreach (var temp in mClassDatas)
            {
                generate_js_class(temp);
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

                output.AppendLine("const msgpack = require('msgpack');");
                foreach (var data in mClassDatas)
                {
                    output.AppendLine($"const Defines = require('./{data.fileName}.js');");
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


        void generate_js_class(ClassFileData data)
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

        void generate_js_pass1(Type _type, StringBuilder _output)
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
                _output.Append("\tconstructor(");
                bool started = false;
                foreach (FieldInfo finfo in fields)
                {
                    if (started)
                    {
                        _output.Append($", {finfo.Name}");
                    }
                    else
                    {
                        started = true;
                        _output.Append(finfo.Name);
                    }
                }
                _output.AppendLine(") {");
                foreach (FieldInfo finfo in _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    _output.AppendLine($"\t\tthis.{finfo.Name} = {finfo.Name};");
                }
                _output.AppendLine("\t}");
                _output.AppendLine("\tInit(packet) {");
                int fcount = 0;
                foreach (FieldInfo finfo in _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (IsArray(finfo))
                    {
                        if (IsClass(finfo))
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
                        if (IsClass(finfo))
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
    var content = msgpack.unpack(data);

    return createPacket(type_name, content);
}}

module.exports.pack = (obj) =>
{{
    var type_name = obj.constructor.name;
    var buf_length = Buffer.alloc(2);
    var buf_name = Buffer.alloc(type_name.length);
    var buf_data = msgpack.pack(obj.ToArray());

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

    } // end of class
}
