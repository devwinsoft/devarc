using System.Text.Json;
using Devarc;

void usage()
{
    Console.WriteLine("[command] [setting.json]");
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
    var setting = JsonSerializer.Deserialize<SettingList>(data);
    if (setting == null)
    {
        usage();
        return;
    }


    foreach (var table in setting.tables)
    {
        switch (table.type)
        {
            case "common":
                foreach (var fileName in table.files)
                {
                    {
                        var builder = new CsTableBuilder();
                        builder.Build(setting.config, fileName, EXPORT_TYPE.All);
                    }
                    {
                        var builder = new JsonTableBuilder();
                        builder.Build(setting.config, fileName, EXPORT_TYPE.All);
                    }
                    {
                        var builder = new UnityTableBuilder();
                        builder.Build(setting.config, fileName);
                    }
                }
                break;
            
            case "client":
                foreach (var fileName in table.files)
                {
                    {
                        var builder = new CsTableBuilder();
                        builder.Build(setting.config, fileName, EXPORT_TYPE.Client);
                    }
                    {
                        var builder = new JsonTableBuilder();
                        builder.Build(setting.config, fileName, EXPORT_TYPE.Client);
                    }
                    {
                        var builder = new UnityTableBuilder();
                        builder.Build(setting.config, fileName);
                    }
                }
                break;
            
            case "server":
                foreach (var fileName in table.files)
                {
                    {
                        var builder = new CsTableBuilder();
                        builder.Build(setting.config, fileName, EXPORT_TYPE.Server);
                    }
                    {
                        var builder = new JsonTableBuilder();
                        builder.Build(setting.config, fileName, EXPORT_TYPE.Server);
                    }
                    {
                        var builder = new MySqlTableBuilder();
                        builder.Build(fileName);
                    }
                }
                break;
            
            case "string":
                foreach (var fileName in table.files)
                {
                    {
                        var builder = new LStrTableBuilder();
                        builder.Build(setting.config, fileName);
                    }
                }
                break;

            default:
                break;
        }
    }
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}
