using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Devarc
{
    class SourceData
    {
        public string fileName = string.Empty;
        public List<INamedTypeSymbol> symbols = new List<INamedTypeSymbol>();
    }

    class TypeData
    {
        public TypeKind typeKind;
        public string name;
        public List<KeyValuePair<string, int>> enumValues = new List<KeyValuePair<string, int>>();
        public bool hasFlagsAttribute = false;
    }

    abstract class BaseBuilder
    {
        protected List<SourceData> mSourceDatas = new List<SourceData>();
        protected Dictionary<string, List<INamedTypeSymbol>> mProtocolDatas = new Dictionary<string, List<INamedTypeSymbol>>();

        protected string ToTypeName(ISymbol symbol)
        {
            try
            {
                if (symbol is IFieldSymbol fieldSymbol)
                {
                    string typeName = fieldSymbol.Type.Name;
                    string typeNameEx = fieldSymbol.Type.Name.ToLower();
                    if (symbol is IArrayTypeSymbol)
                    {
                        if (typeNameEx.EndsWith("void"))
                            return "void";
                        if (typeNameEx.Equals("uint64"))
                            return "List<ulong>";
                        if (typeNameEx.Equals("uint32"))
                            return "List<uint>";
                        if (typeNameEx.Equals("uint16"))
                            return "List<ushort>";
                        if (typeNameEx.Equals("int64"))
                            return "List<long>";
                        if (typeNameEx.Equals("int32"))
                            return "List<int>";
                        if (typeNameEx.Equals("int16"))
                            return "List<short>";
                        if (typeNameEx.Equals("float") || typeNameEx.Equals("single"))
                            return "List<float>";
                        if (typeNameEx.Equals("byte"))
                            return "List<byte>";
                        if (typeNameEx.Equals("string"))
                            return "List<string>";
                        return $"List<{typeName.Substring(0, typeName.Length - 2)}>";
                    }
                    else
                    {
                        if (typeNameEx.EndsWith("void"))
                            return "void";
                        if (typeNameEx.Equals("boolean"))
                            return "bool";
                        if (typeNameEx.Equals("uint64"))
                            return "ulong";
                        if (typeNameEx.Equals("uint32"))
                            return "uint";
                        if (typeNameEx.Equals("uint16"))
                            return "ushort";
                        if (typeNameEx.Equals("int64"))
                            return "long";
                        if (typeNameEx.Equals("int32"))
                            return "int";
                        if (typeNameEx.Equals("int16"))
                            return "short";
                        if (typeNameEx.Equals("float") || typeNameEx.Equals("single"))
                            return "float";
                        if (typeNameEx.Equals("byte"))
                            return "byte";
                        if (typeNameEx.Equals("string"))
                            return "string";
                        return fieldSymbol.Type.Name;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        
        void collect_protocol_namespaces(INamespaceSymbol ns)
        {
            foreach (var member in ns.GetMembers())
            {
                if (member is INamespaceSymbol nestedNs)
                {
                    collect_protocol_symbols(nestedNs);
                }
            }
        }
        
        protected void collect_protocol_symbols(INamespaceSymbol ns)
        {
            switch (ns.Name.ToLower())
            {
                case "internal":
                case "system":
                    return;
                default:
                    break;
            }
            
            foreach (var member in ns.GetMembers())
            {
                if (member is INamedTypeSymbol symbol)
                {
                    List<INamedTypeSymbol> list;
                    if (mProtocolDatas.TryGetValue(ns.Name, out list) == false)
                    {
                        list = new List<INamedTypeSymbol>();
                        mProtocolDatas[ns.Name] = list;
                    }
                    list.Add(symbol);
                }
            }
        }
        

        protected void collect_source_classes(INamespaceSymbol ns, SourceData data)
        {
            foreach (var member in ns.GetTypeMembers())
            {
                if (member is INamedTypeSymbol symbol)
                {
                    if (symbol.ContainingAssembly.Identity.Name.StartsWith("System."))
                        continue;
                    data.symbols.Add(symbol);
                }
            }
        }

        protected bool collect_source_datas(string filePath)
        {
            string srcData = File.ReadAllText(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);

            var syntaxTree = CSharpSyntaxTree.ParseText(srcData);
            var refer = new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location)
            };
            var compilation = CSharpCompilation.Create(
                assemblyName: $"{fileName}.dll",
                syntaxTrees: new[] { syntaxTree },
                references: refer,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);
            if (!result.Success)
            {
                foreach (var diagnostic in result.Diagnostics)
                    Console.WriteLine(diagnostic.ToString());
                return false;
            }

            SourceData data = new SourceData();
            data.fileName = fileName;
            collect_source_classes(compilation.GlobalNamespace, data);
            mSourceDatas.Add(data);
    
            return true;
        }
        
        protected void collect_protocol_datas(ConfigData cfg)
        {
            var syntaxTrees = new SyntaxTree[cfg.sources.Count + cfg.protocols.Count];
            int count = 0;
            for (int i = 0; i < cfg.sources.Count; i++)
            {
                string code = File.ReadAllText(cfg.sources[i]);
                syntaxTrees[count++] = CSharpSyntaxTree.ParseText(code);
            }
            for (int i = 0; i < cfg.protocols.Count; i++)
            {
                string code = File.ReadAllText(cfg.protocols[i]);
                syntaxTrees[count++] = CSharpSyntaxTree.ParseText(code);
            }
            var refer = new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location)
            };
            var compilation = CSharpCompilation.Create(
                assemblyName: "protocols",
                syntaxTrees: syntaxTrees,
                references: refer,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);
            if (!result.Success)
            {
                foreach (var diagnostic in result.Diagnostics)
                    Console.WriteLine(diagnostic.ToString());
                return;
            }
            
            collect_protocol_namespaces(compilation.GlobalNamespace);
        }
    }
}
