using System.Collections.Generic;
using System.Reflection;
using Nickel;
using TuckerTheSaboteur;

namespace TuckerMod.tuckerArtifacts.Duos;

public class GrouchoStache : Artifact, IRegisterableArtifact
{
	public static void Register(IModHelper helper)
	{
		if (Main.Instance.DuoArtifactsApi == null) return;

		helper.Content.Artifacts.RegisterArtifact("GrouchoStache", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Main.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/icons/duos/GrouchoStache.png")).Sprite,
			Name = Main.Instance.AnyLocalizations.Bind(["artifact", "duo", "GrouchoStache", "name"]).Localize,
			Description = Main.Instance.AnyLocalizations.Bind(["artifact", "duo", "GrouchoStache", "description"]).Localize
		});
		Main.Instance.DuoArtifactsApi.RegisterDuoArtifact<GrouchoStache>([Deck.riggs, Main.Instance.TuckerDeck.Deck]);
	}

	public override void OnTurnStart(State state, Combat combat)
	{
		int evade = state.ship.Get(Status.evade);
		if (state.ship.Get(Status.lockdown) > 0 && evade > 0) {
			combat.Queue(new AStatus {
				status = Status.tempShield,
				statusAmount = evade,
				targetPlayer = true,
				artifactPulse = Key()
			});
		}
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		.. StatusMeta.GetTooltips(Status.lockdown, 1),
		.. StatusMeta.GetTooltips(Status.evade, 1),
		.. StatusMeta.GetTooltips(Status.tempShield, 1),
	];
}