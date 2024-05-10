using Newtonsoft.Json;

namespace Oxide.Ext.BattleMetricsFramework;

[Serializable]
public class BattleMetricsServer
{
    [JsonProperty("id")]
    public string ID;

    [JsonProperty("name")]
    public string Name;

    [JsonProperty("ip")]
    public string IP;

    [JsonProperty("port")]
    public int Port;

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(ID) && !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(IP) && Port > 0;
    }
}