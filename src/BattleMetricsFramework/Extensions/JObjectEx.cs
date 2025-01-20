using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using Oxide.Core;

namespace Oxide.Ext.BattleMetricsFramework;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class JObjectEx
{
    public static bool TryParseBattleMetricsPlayerID(this JObject jObject, out string playerId)
    {
        playerId = null;

        if (jObject == null)
            return false;

        if (!jObject.TryGetValue("data", out JToken token))
            return false;

        if (token.Type != JTokenType.Array)
            return false;

        var data = (JArray)token;

        JToken value = data.FirstOrDefault();
        if (value == null) return false;

        playerId = value.Value<string>("id");
        return !string.IsNullOrWhiteSpace(playerId);
    }

    public static IEnumerable<BattleMetricsServer> GetBattleMetricsServers(this JObject jObject)
    {
        if (jObject == null)
            yield break;

        if (!jObject.TryGetValue("data", out JToken token))
            yield break;

        if (token.Type != JTokenType.Array)
            yield break;

        foreach (JToken value in (JArray)token)
        {
            var server = new BattleMetricsServer();
            server.ID = value?.Value<string>("id");

            var attributes = value?.Value<JObject>("attributes");
            server.Name = attributes?.Value<string>("name");
            server.IP = attributes?.Value<string>("ip");
            server.Port = attributes?.Value<int>("port") ?? 0;

            if (server.IsValid())
                yield return server;
        }
    }

    public static BattleMetricsSession GetBattleMetricsSession(this JObject jObject)
    {
        var session = new BattleMetricsSession();

        if (jObject == null)
            return session;

        if (!jObject.TryGetValue("data", out JToken token))
            return session;

        if (token.Type != JTokenType.Array)
            return session;

        foreach (JToken value in (JArray)token)
        {
            var relationships = value.Value<JObject>("relationships");
            var server = relationships?.Value<JObject>("server");
            var data = server?.Value<JObject>("data");
            string id = data?.Value<string>("id");

            if (!string.IsNullOrEmpty(id))
                session.Servers.Add(id);
        }

        return session;
    }

    internal static bool TryCalculatePlaytimeThreaded(this JObject jObject, Action<int> callback)
    {
        if (jObject == null)
            return false;

        if (!jObject.TryGetValue("included", out JToken token))
            return false;

        if (token.Type != JTokenType.Array)
            return false;

        Task.Run(() =>
        {
            int playtime = 0;

            foreach (JToken value in (JArray)token)
            {
                var meta = value?.Value<JObject>("meta");
                playtime += meta?.Value<int>("timePlayed") ?? 0;
            }

            Interface.Oxide.NextTick(() => callback?.Invoke(playtime));
        });

        return true;
    }
}