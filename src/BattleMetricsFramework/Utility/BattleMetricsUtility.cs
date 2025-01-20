using System.Collections;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

namespace Oxide.Ext.BattleMetricsFramework;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class BattleMetricsUtility
{
    public static readonly Dictionary<ulong, float> PlayerToPlaytime = new();

    public static void GetPlayerPlaytime(string token, ulong userID, Action<float> callback)
    {
        if (PlayerToPlaytime.TryGetValue(userID, out float playtime))
            callback?.Invoke(playtime);

        // TODO: query playtime on BM
    }

    /// <summary>
    /// Returns a list of servers owned by an organization.
    /// </summary>
    /// <param name="token">BattleMetrics token.</param>
    /// <param name="organizationID">BattleMetrics organization's id.</param>
    /// <param name="callback">The callback to invoke on completion.</param>
    public static void GetOrganizationServers(string token, string organizationID, Action<JObject> callback)
    {
        IEnumerator task = GetOrganizationServersInternal(token, organizationID, callback);
        CoroutineUtility.StartCoroutine(task);
    }

    private static IEnumerator GetOrganizationServersInternal(string token, string organizationID, Action<JObject> callback)
    {
        string url = $"https://api.battlemetrics.com/servers?filter[organizations]={organizationID}";
        yield return GetPlusParseAsJObject(token, url, callback);
    }

    /// <summary>
    /// Returns the player information of BattleMetrics.
    /// </summary>
    /// <param name="token">BattleMetrics token.</param>
    /// <param name="userID">User's steam64.</param>
    /// <param name="callback">The callback to invoke on completion.</param>
    public static void GetPlayer(string token, ulong userID, Action<JObject> callback)
    {
        IEnumerator task = GetBattleMetricsPlayer(token, userID, callback);
        CoroutineUtility.StartCoroutine(task);
    }

    private static IEnumerator GetBattleMetricsPlayer(string token, ulong userID, Action<JObject> callback)
    {
        string url = $"https://api.battlemetrics.com/players?filter[search]={userID}";
        yield return GetPlusParseAsJObject(token, url, callback);
    }

    /// <summary>
    /// Returns the server information of BattleMetrics.
    /// </summary>
    /// <param name="token">BattleMetrics token.</param>
    /// <param name="userID">User's steam64.</param>
    /// <param name="callback">The callback to invoke on completion.</param>
    public static void GetPlayerSession(string token, ulong userID, Action<JObject> callback)
    {
        GetPlayer(token, userID, jPlayer =>
        {
            if (jPlayer.TryParseBattleMetricsPlayerID(out string playerID))
            {
                IEnumerator task = GetPlayerSessionInternal(token, playerID, callback);
                CoroutineUtility.StartCoroutine(task);
            }
            else
            {
                callback?.Invoke(null);
            }
        });
    }

    private static IEnumerator GetPlayerSessionInternal(string token, string playerId, Action<JObject> callback)
    {
        string url = $"https://api.battlemetrics.com/players/{playerId}/relationships/sessions";
        yield return GetPlusParseAsJObject(token, url, callback);
    }

    /// <summary>
    /// Returns the player's current server, if any, otherwise returns null.
    /// </summary>
    /// <param name="token">BattleMetrics token.</param>
    /// <param name="userID">User's steam64.</param>
    /// <param name="callback">The callback to invoke on completion.</param>
    public static void GetPlayerCurrentServer(string token, ulong userID, Action<string> callback)
    {
        GetPlayer(token, userID, jPlayer =>
        {
            if (jPlayer.TryParseBattleMetricsPlayerID(out string playerID))
            {
                IEnumerator task = GetPlayerSessionInternal(token, playerID, jSession =>
                {
                    BattleMetricsSession session = jSession.GetBattleMetricsSession();
                    string lastServer = session.Servers.FirstOrDefault();

                    if (string.IsNullOrEmpty(lastServer))
                    {
                        callback?.Invoke(null);
                    }
                    else
                    {
                        IEnumerator task = GetPlayerCurrentServerInternal(token, playerID, lastServer, callback);
                        CoroutineUtility.StartCoroutine(task);
                    }
                });

                CoroutineUtility.StartCoroutine(task);
            }
            else
            {
                callback?.Invoke(null);
            }
        });
    }

    private static IEnumerator GetPlayerCurrentServerInternal(string token, string playerID, string serverID, Action<string> callback)
    {
        string url = $"https://api.battlemetrics.com/players/{playerID}/servers/{serverID}";
        yield return GetPlusParseAsJObject(token, url, jObject =>
        {
            JToken data = jObject.GetValue("data");
            var attributes = data?.Value<JObject>("attributes");
            bool isOnline = attributes?.Value<bool>("online") ?? false;

            if (!isOnline)
                serverID = null;

            callback?.Invoke(serverID);
        });
    }

    private static IEnumerator GetPlusParseAsJObject(string token, string url, Action<JObject> callback)
    {
        JObject jObject = null;

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", $"Bearer {token}");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                jObject = JObject.Parse(request.downloadHandler.text);
        }

        callback?.Invoke(jObject);
    }
}