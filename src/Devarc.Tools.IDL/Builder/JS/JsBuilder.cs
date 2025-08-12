using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace Devarc
{
    internal class JsBuilder : BaseJsBuilder
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

            foreach (var sourceData in mSourceDatas)
            {
                generate_js_code(sourceData);
            }

            foreach (var protocolData in mProtocolDatas)
            {
                StringBuilder output = new StringBuilder();
                string namespaceName = protocolData.Key;

                output.AppendLine("const msgpack = require('msgpack-lite');");
                /*
                foreach (var source in mSourceDatas)
                {
                    output.AppendLine($"const {namespaceName} = require('./{source.fileName}.js');");
                    foreach (var symbol in source.symbols)
                    {
                        output.AppendLine($"const {symbol.Name} = {namespaceName}.{symbol.Name};");
                    }
                }
                output.AppendLine("const mHandlers = {};");
                */

                foreach (var symbol in protocolData.Value)
                {
                    generate_js_class(symbol, output);
                }

                int line = 0;
                output.AppendLine("module.exports =");
                foreach (var symbol in protocolData.Value)
                {
                    if (line == 0)
                        output.AppendLine($"{{ {symbol.Name}");
                    else
                        output.AppendLine($", {symbol.Name}");
                    line++;
                }

                output.AppendLine("}");
                output.AppendLine("");

                generate_js_packet(protocolData.Value, output);
                
                string fileName = $"{namespaceName}.js";
                File.WriteAllText(fileName, output.ToString());
            }
            
            // Move & Delete files.
            foreach (var sourceData in mSourceDatas)
            {
                string fileName = $"{sourceData.fileName}.js";
                foreach (var outputDir in cfg.source_outputs)
                {
                    Utils.EnsureDirectory(outputDir);
                    File.Copy(fileName, Path.Combine(outputDir, fileName), true);
                }
                File.Delete(fileName);
            }
            foreach (var protocolData in mProtocolDatas)
            {
                string namespaceName = protocolData.Key;
                string fileName = $"{namespaceName}.js";
                foreach (var outputDir in cfg.source_outputs)
                {
                    File.Copy(fileName, Path.Combine(outputDir, fileName), true);
                }
                File.Delete(fileName);
            }

        }

    } // end of class
}
