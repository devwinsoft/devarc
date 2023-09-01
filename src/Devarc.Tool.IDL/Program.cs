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
                    var builder = new CsBuilder();
                    builder.Build(args[1], files);
                }
                else if (args.Contains<string>("-js"))
                {
                    var builder = new JsBuilder();
                    builder.Build(args[1], files);
                }
                else if (args.Contains<string>("-cs-def"))
                {
                    var builder = new CsDefBuilder();
                    builder.Build(args[1]);
                }
                else if (args.Contains<string>("-js-def"))
                {
                    var builder = new JsDefBuilder();
                    builder.Build(args[1]);
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
            Console.WriteLine("[command] -[cs/js/cs-def/js-def] [protocol.idl] [class.idl, ...]");
        }
    }
}
