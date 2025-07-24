
// See https://aka.ms/new-console-template for more information
//var args = Environment.GetCommandLineArgs();

using System.Text.Json;
using Devarc;

void usage()
{
    Console.WriteLine("[command] [input.json]");
}

try
{
    if (args.Length < 1)
    {
        usage();
        return;
    }
    
    var cfgFileName = args[0];
    if (File.Exists(cfgFileName) == false)
    {
        usage();
        return;
    }
    
    var data = File.ReadAllText(cfgFileName);
    var info = JsonSerializer.Deserialize<ConfigList>(data);
    if (info == null)
    {
        usage();
        return;
    }
    
    foreach (var obj in info.list)
    {
        switch (obj.language)
        {
            case "cs":
            {
                var builder = new CsBuilder();
                builder.Build(obj);
                break;
            }
            case "js":
            {
                var builder = new JsBuilder();
                builder.Build(obj);
                break;
            }
            default:
                usage();
                break;
        }
    }
}
catch (Exception e)
{
    Console.WriteLine(e);
}
