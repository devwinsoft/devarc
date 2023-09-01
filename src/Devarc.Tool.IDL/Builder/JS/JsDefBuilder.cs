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
    internal class JsDefBuilder : BaseJsBuilder
    {
        public void Build(string inputFile)
        {
            mAssemDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            mBakCurDir = Directory.GetCurrentDirectory();
            mWorkingDir = Path.GetFullPath(".\\");
            Directory.SetCurrentDirectory(mWorkingDir);

            compileClassFileData(inputFile);
            foreach (var temp in mClassDatas)
            {
                generate_js_class(temp);
            }

            Directory.SetCurrentDirectory(mBakCurDir);
        }

    }
}
