using System.Reflection;
using Nickel;

namespace TuckerTheSaboteur.Artifacts;

public class HoloWall : Artifact, IRegisterableArtifact
{
	public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("HoloWall", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Main.Instance.TuckerDeck.Deck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/icons/Holo-Wall.png")).Sprite,
			Name = Main.Instance.AnyLocalizations.Bind(["artifact", "HoloWall", "name"]).Localize,
			Description = Main.Instance.AnyLocalizations.Bind(["artifact", "HoloWall", "description"]).Localize
		});
	}
    
    public override string Description() => "Gain 3 <c=status>Buffer</c> on the first turn.";
    public override void OnCombatStart(State state, Combat combat)
    {
        this.Pulse();
        combat.QueueImmediate(new AStatus {
            targetPlayer = true,
            status = Main.Instance.BufferStatus.Status,
            statusAmount = 3
        });
    }
}
