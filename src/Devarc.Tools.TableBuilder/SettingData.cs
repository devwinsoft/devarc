namespace Devarc
{
    [System.Flags]
    public enum EXPORT_TYPE
    {
        None,
        Client = 1 << 1,
        Server = 1 << 2,
        All = Client | Server
    }

    [System.Serializable]
    public class SettingConfig
    {
        public string client_script { get; set; }
        public string client_bundle { get; set; }
        public string client_resource { get; set; }
        
        public string language_bundle { get; set; }
        public string language_resource { get; set; }
        
        public string server_script { get; set; }
        public string server_database { get; set; }
    }
    
    [System.Serializable]
    public class SettingTable
    {
        public string type { get; set; }
        public List<string> files { get; set; }
    }


    [System.Serializable]
    public class SettingList
    {
        public SettingConfig config { get; set; }
        public List<SettingTable> tables { get; set; }
    }
}

