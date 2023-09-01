using Microsoft.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Devarc
{
    internal class CsBuilder : BaseCsBuilder
    {
        public void Build(string protocolFile, params string[] classFiles)
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

    }
}
