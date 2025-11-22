using System.Collections.Generic;
using System.Reflection;
using Nickel;
using TuckerTheSaboteur;

namespace TuckerMod.tuckerArtifacts.Duos;

public class PetBed : Artifact, IRegisterableArtifact, ITyAndSashaApi.IHook
{
	public static void Register(IModHelper helper)
	{
		if (Main.Instance.DuoArtifactsApi == null || Main.Instance.TyAndSashaApi == null) return;

		helper.Content.Artifacts.RegisterArtifact("PetBed", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Main.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/icons/duos/PetBed.png")).Sprite,
			Name = Main.Instance.AnyLocalizations.Bind(["artifact", "duo", "PetBed", "name"]).Localize,
			Description = Main.Instance.AnyLocalizations.Bind(["artifact", "duo", "PetBed", "description"]).Localize
		});
		Main.Instance.DuoArtifactsApi.RegisterDuoArtifact<PetBed>([Main.Instance.TyAndSashaApi.TyDeck, Main.Instance.TuckerDeck.Deck]);
	}

	public int AffectX(ITyAndSashaApi.IHook.IAffectXArgs args) {
		var s = args.State;
		if (s.ship.Get(Status.lockdown) > 0 || s.ship.Get(Status.engineStall) > 0) return 2;
		return 0;
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		.. StatusMeta.GetTooltips(Status.lockdown, 1),
		.. StatusMeta.GetTooltips(Status.engineStall, 1),
	];
}