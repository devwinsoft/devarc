using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                else if (args.Contains<string>("-sql"))
                {
                    var builder = new MySqlBuilder();
                    builder.Build(fileName);
                }
                else if (args.Contains<string>("-unity"))
                {
                    var builder = new UnityBuilder();
                    builder.Build(fileName);
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
