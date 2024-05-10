# BattleMetrics Framework
BattleMetrics framework for [Rust](https://store.steampowered.com/app/252490/Rust/) using the [Oxide/uMod](https://umod.org) extension platforms to expose useful api methods for plugins.

## Getting Started
1. Grab the Oxide.Ext.BattleMetricsFramework.dll from latest release
2. Put the DLL into `RustDedicated_Data\Managed` folder
3. Restart the server

## Usage
```csharp
using Oxide.Ext.BattleMetricsFramework;

// some code
BattleMetricsUtility.GetOrganizationServers(token, organizationID, jObject =>
{
    foreach (BattleMetricsServer bmServer in jObject.GetBattleMetricsServers())
        Debug.Log($"{bmServer.ID} - {bmServer.Name}");
});
```
