using System.Collections.Generic;
using System.Reflection;
using Nickel;
using TuckerTheSaboteur;
using TuckerTheSaboteur.actions;

namespace TuckerMod.tuckerArtifacts.Duos;

public class Whetstone : Artifact, IRegisterableArtifact
{
	public static void Register(IModHelper helper)
	{
		if (Main.Instance.DuoArtifactsApi == null) return;

		helper.Content.Artifacts.RegisterArtifact("Whetstone", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Main.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/icons/duos/Whetstone.png")).Sprite,
			Name = Main.Instance.AnyLocalizations.Bind(["artifact", "duo", "Whetstone", "name"]).Localize,
			Description = Main.Instance.AnyLocalizations.Bind(["artifact", "duo", "Whetstone", "description"]).Localize
		});
		Main.Instance.DuoArtifactsApi.RegisterDuoArtifact<Whetstone>([Deck.shard, Main.Instance.TuckerDeck.Deck]);
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		.. new ABluntAttack().GetTooltips(DB.fakeState),
		.. StatusMeta.GetTooltips(Status.shard, 3)
	];
}