using Newtonsoft.Json;

namespace Oxide.Ext.BattleMetricsFramework;

[Serializable]
public class BattleMetricsSession
{
    [JsonProperty("servers")]
    public List<string> Servers = new();
}