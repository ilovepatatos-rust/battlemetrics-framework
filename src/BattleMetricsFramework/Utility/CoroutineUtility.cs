using System.Collections;

namespace Oxide.Ext.BattleMetricsFramework;

internal static class CoroutineUtility
{
    public static void StartCoroutine(IEnumerator enumerator)
    {
        ServerMgr.Instance.StartCoroutine(enumerator);
    }
}