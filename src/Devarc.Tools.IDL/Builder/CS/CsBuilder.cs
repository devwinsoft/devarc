using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Devarc
{
    internal class CsBuilder : BaseBuilder
    {
        public void Build(ConfigData cfg)
        {
            foreach (var source in cfg.sources)
            {
                if (File.Exists(source) == false)
                {
                    Console.WriteLine($"Cannot find source file: {source}");
                    return;
                }
                collect_source_datas(source);
            }
            collect_protocol_datas(cfg);

            foreach (var data in mSourceDatas)
            {
                generate_cs_class(cfg, data);
            }
            
            foreach (var temp in mProtocolDatas)
            {
                var output = new StringBuilder();
                var namespaceName = temp.Key;
                var symbols = temp.Value;
                var outputPath = $"{namespaceName}.cs";

                output.AppendLine("using System.Collections.Generic;");
                output.AppendLine("using MessagePack;");
                output.AppendLine("using Devarc;");
                output.AppendLine("");
                output.AppendLine($"namespace {namespaceName}");
                output.AppendLine("{");
                foreach (var symbol in symbols)
                {
                    append_cs_protocol(symbol, output);
                }
                output.AppendLine("};");

                File.WriteAllText(outputPath, output.ToString());
                foreach (var outputDir in cfg.protocol_outputs)
                {
                    File.Copy(outputPath, Path.Combine(outputDir, outputPath), true);
                }
                File.Delete(outputPath);
            }
        }

        void generate_cs_class(ConfigData cfg, SourceData data)
        {
            string fileName = Path.GetFileName(data.fileName + ".cs");
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using MessagePack;");
            sb.AppendLine("");
            sb.AppendLine($"namespace Devarc");
            sb.AppendLine("{");
            foreach (var symbol in data.symbols)
            {
                append_cs_protocol(symbol, sb);
            }

            sb.AppendLine("};");

            File.WriteAllText(data.fileName, sb.ToString());
            foreach (var outputDir in cfg.source_outputs)
            {
                Utils.EnsureDirectory(outputDir);
                File.Copy(data.fileName, Path.Combine(outputDir, fileName), true);
            }
            File.Delete(data.fileName);
        }


        void append_cs_protocol(ITypeSymbol symbol, StringBuilder output)
        {
            if (symbol.TypeKind == TypeKind.Class)
            {
                output.AppendLine($"\t[MessagePackObject]");
                output.AppendLine($"\tpublic partial class {symbol.Name} : BaseTableElement<{symbol.Name}>, ITableElement<{symbol.Name}>");
                output.AppendLine("\t{");
                int fcount = 0;
                foreach (var member in symbol.GetMembers())
                {
                    var typeName = ToTypeName(member);
                    if (string.IsNullOrEmpty(typeName))
                        continue;
                    
                    output.AppendLine($"\t\t[Key({fcount})]");
                    if (member is IArrayTypeSymbol)
                        output.AppendLine(string.Format("\t\tpublic {0,-20} {1} = new {0}();", typeName, member.Name));
                    else if (member is INamedTypeSymbol)
                        output.AppendLine(string.Format("\t\tpublic {0,-20} {1} = new {0}();", typeName, member.Name));
                    else
                        output.AppendLine(string.Format("\t\tpublic {0,-20} {1};", typeName, member.Name));
                    fcount++;
                }
                output.AppendLine("\t}");
                output.AppendLine("");
            }
            else if (symbol.TypeKind == TypeKind.Enum)
            {
                bool hasFlags = symbol.GetAttributes().Any(attr =>
                    attr.AttributeClass?.ToDisplayString() == "System.FlagsAttribute");
                if (hasFlags)
                    output.AppendLine("\t[System.Flags]");
                output.AppendLine($"\tpublic enum {symbol.Name}");
                output.AppendLine("\t{");
                foreach (var member in symbol.GetMembers().OfType<IFieldSymbol>())
                {
                    if (member.ConstantValue != null)
                    {
                        output.AppendLine(string.Format("\t\t{0,-20} = {1},", member.Name, member.ConstantValue));
                    }
                }
                output.AppendLine("\t}");
                output.AppendLine("");
            }
        }

    } // end of class
}
