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
    internal class CsDefBuilder : BaseCsBuilder
    {
        public void Build(string inputFile)
        {
            mAssemDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            mBakCurDir = Directory.GetCurrentDirectory();
            mWorkingDir = Path.GetFullPath(".\\");
            Directory.SetCurrentDirectory(mWorkingDir);

            // Exeption handling
            if (File.Exists(inputFile) == false)
            {
                Console.WriteLine($"Cannot find base file: {inputFile}");
                Directory.SetCurrentDirectory(mBakCurDir);
                return;
            }

            compileClassFileData(inputFile);
            foreach (var temp in mClassDatas)
            {
                generate_cs_class(temp);
            }

            Directory.SetCurrentDirectory(mBakCurDir);
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

    }
}
