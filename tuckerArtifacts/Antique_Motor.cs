using TuckerTheSaboteur;

namespace TuckertheSabotuer.Artifacts
{

    [ArtifactMeta(pools = new ArtifactPool[] { ArtifactPool.Boss })]
    public class AntiqueMotor : Artifact
    {
        public override string Description() => "Gain 1 extra <c=energy>ENERGY</c> every turn. <c=downside>Gain 1</c> <c=status>FUEL LEAK</c> <c=downside>on the first turn.";
        public override void OnCombatStart(State state, Combat combat)
        {
            this.Pulse();
            combat.QueueImmediate(new AStatus()
            {
                targetPlayer = true,
                status = (Status)MainManifest.statuses["fuel_leak"].Id,
                statusAmount = 1
            });
        }
        public override void OnReceiveArtifact(State state) => ++state.ship.baseEnergy;
        public override void OnRemoveArtifact(State state) => --state.ship.baseEnergy;
    }
}
