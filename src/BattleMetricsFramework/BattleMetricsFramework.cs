using JetBrains.Annotations;
using Oxide.Core;
using Oxide.Core.Extensions;

namespace Oxide.Ext.BattleMetricsFramework;

[UsedImplicitly]
public class BattleMetricsFramework : Extension
{
    private static readonly VersionNumber s_extensionVersion = new(1, 0, 0);

    public override string Name => "BattleMetricsFramework";
    public override string Author => "Ilovepatatos";
    public override VersionNumber Version => s_extensionVersion;

    public override bool SupportsReloading => true;

    public BattleMetricsFramework(ExtensionManager manager) : base(manager) { }

    public override IEnumerable<string> GetPreprocessorDirectives()
    {
        yield return "BATTLE_METRICS_FRAMEWORK";
    }
}