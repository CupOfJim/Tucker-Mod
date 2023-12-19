using TuckerTheSaboteur;

namespace TuckertheSabotuer.Artifacts
{

    [ArtifactMeta(pools = new ArtifactPool[] { ArtifactPool.Common })]
    public class HoloWall : Artifact
    {
        public override string Description() => "Gain 3 <c=status>Buffer</c> on the first turn.";
        public override void OnCombatStart(State state, Combat combat)
        {
            this.Pulse();
            combat.QueueImmediate(new AStatus()
            {
                targetPlayer = true,
                status = (Status)MainManifest.statuses["buffer"].Id,
                statusAmount = 3
            });
        }
    }
}
