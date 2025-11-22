using System.Collections.Generic;
using System.Reflection;
using Nickel;
using TuckerTheSaboteur;

namespace TuckerMod.tuckerArtifacts.Duos;

public class WireTap : Artifact, IRegisterableArtifact
{
	public static void Register(IModHelper helper)
	{
		if (Main.Instance.DuoArtifactsApi == null) return;

		helper.Content.Artifacts.RegisterArtifact("WireTap", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Main.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/icons/duos/WireTap.png")).Sprite,
			Name = Main.Instance.AnyLocalizations.Bind(["artifact", "duo", "WireTap", "name"]).Localize,
			Description = Main.Instance.AnyLocalizations.Bind(["artifact", "duo", "WireTap", "description"]).Localize
		});
		Main.Instance.DuoArtifactsApi.RegisterDuoArtifact<WireTap>([Deck.goat, Main.Instance.TuckerDeck.Deck]);
	}

	public override void OnTurnEnd(State state, Combat combat)
	{
		if (state.ship.Get(Status.lockdown) > 0 || state.ship.Get(Status.engineStall) > 0) combat.Queue(new AStatus {
			status = Status.droneShift,
			statusAmount = 1,
			targetPlayer = true,
			artifactPulse = Key()
		});
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		.. StatusMeta.GetTooltips(Status.lockdown, 1),
		.. StatusMeta.GetTooltips(Status.engineStall, 1),
		.. StatusMeta.GetTooltips(Status.droneShift, 1),
	];
}