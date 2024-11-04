using System.Reflection;
using Nickel;

namespace TuckerTheSaboteur.Artifacts;

public class AntiqueMotor : Artifact, IRegisterableArtifact
{
	public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("AntiqueMotor", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Main.Instance.TuckerDeck.Deck,
				pools = [ArtifactPool.Boss]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/icons/Antique_Motor.png")).Sprite,
			Name = Main.Instance.AnyLocalizations.Bind(["artifact", "AntiqueMotor", "name"]).Localize,
			Description = Main.Instance.AnyLocalizations.Bind(["artifact", "AntiqueMotor", "description"]).Localize
		});
	}

	public override void OnTurnStart(State state, Combat combat)
	{
        Pulse();
        combat.QueueImmediate(new AStatus {
            targetPlayer = true,
            status = Main.Instance.FuelLeakStatus.Status,
            statusAmount = 1
        });
    }
    public override void OnReceiveArtifact(State state) => ++state.ship.baseEnergy;
    public override void OnRemoveArtifact(State state) => --state.ship.baseEnergy;
}
