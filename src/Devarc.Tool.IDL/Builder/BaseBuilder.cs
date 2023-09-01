using Microsoft.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;


namespace Devarc
{
    class ClassFileData
    {
        public string fileName;
        public string filePath;
        public Type[] types;
    }


    abstract class BaseBuilder
    {
        protected string mAssemDir;
        protected string mWorkingDir;
        protected string mBakCurDir;
        protected List<ClassFileData> mClassDatas = new List<ClassFileData>();
        protected Dictionary<string, List<Type>> mNamespaceDatas = new Dictionary<string, List<Type>>();

        protected bool IsArray(FieldInfo finfo) => finfo.FieldType.IsArray;
        protected bool IsArray(Type type) => type.IsArray;
        protected bool IsClass(FieldInfo finfo) => IsClass(finfo.FieldType);
        protected bool IsClass(Type type) => type.IsClass && !type.Name.ToLower().Contains("string");
        protected bool IsEnum(FieldInfo field) => field.FieldType.IsEnum;
        protected bool IsEnum(Type type) => type.IsEnum;


        protected string ToTypeName(FieldInfo finfo)
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


        protected void registerTypeToNamespace(Type type)
        {
            List<Type> list = null;
            if (mNamespaceDatas.TryGetValue(type.Namespace, out list) == false)
            {
                list = new List<Type>();
                mNamespaceDatas[type.Namespace] = list;
            }
            list.Add(type);
        }


        protected bool compileClassFileData(string classFilePath)
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
                if (cr.Errors.Count == 0)
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

    }
}
