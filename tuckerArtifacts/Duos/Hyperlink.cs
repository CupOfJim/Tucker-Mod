using System.Collections.Generic;
using System.Reflection;
using Nickel;
using TuckerTheSaboteur;
using TuckerTheSaboteur.cards;

namespace TuckerMod.tuckerArtifacts.Duos;

public class Hyperlink : Artifact, IRegisterableArtifact
{
	public static void Register(IModHelper helper)
	{
		if (Main.Instance.DuoArtifactsApi == null) return;

		helper.Content.Artifacts.RegisterArtifact("Hyperlink", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Main.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/icons/duos/Hyperlink.png")).Sprite,
			Name = Main.Instance.AnyLocalizations.Bind(["artifact", "duo", "Hyperlink", "name"]).Localize,
			Description = Main.Instance.AnyLocalizations.Bind(["artifact", "duo", "Hyperlink", "description"]).Localize
		});
		Main.Instance.DuoArtifactsApi.RegisterDuoArtifact<Hyperlink>([Deck.catartifact, Main.Instance.TuckerDeck.Deck]);
	}

	public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
	{
		if (Main.Instance.EssentialsApi!.IsExeCardType(card.GetType())) combat.Queue(new AAddCard {
			card = new Misdirection(),
			destination = CardDestination.Hand,
			amount = 1,
			artifactPulse = Key()
		});
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		new TTCard {
			card = new Misdirection()
		}
	];
}