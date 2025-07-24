namespace Devarc
{
    [System.Serializable]
    public class ConfigData
    {
        public string language { get; set; }
        public List<string> sources { get; set; }
        public List<string> source_outputs { get; set; }
        public List<string> protocols { get; set; }
        public List<string> protocol_outputs { get; set; }
    }

    [System.Serializable]
    public class ConfigList
    {
        public List<ConfigData> list { get; set; }
    }
}
