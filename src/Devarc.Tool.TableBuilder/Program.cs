using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devarc.TableBuilder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                string fileName = args[1];
                if (args.Contains<string>("-json"))
                {
                    var builder = new JsonBuilder();
                    builder.Build(fileName);
                }
                else if (args.Contains<string>("-cs"))
                {
                    var builder = new CsBuilder();
                    builder.Build(fileName);
                }
                else if (args.Contains<string>("-js"))
                {
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
            Console.WriteLine("[command] -[json/cs/js] [*.xlsx]");
        }
    }
}
