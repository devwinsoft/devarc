using System;
using System.Linq;

namespace Devarc
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                string[] files = new string[args.Length - 2];
                Array.Copy(args, 2, files, 0, args.Length - 2);

                if (args.Contains<string>("-cs"))
                {
                    Builder compiler = new Builder();
                    compiler.Build_CS(args[1], files);
                }
                else if (args.Contains<string>("-js"))
                {
                    Builder compiler = new Builder();
                    compiler.Build_JS(args[1], files);
                }
                else
                {
                    usage();
                }
            }
            else
            {
                usage();
            }
        }

        static void usage()
        {
            Console.WriteLine("[command] -[cs/js] [protocol.idl] [class.idl, ...]");
        }
    }
}
