using System.Collections.Generic;
using System.Reflection;
using Nickel;
using TuckerTheSaboteur;
using TuckerTheSaboteur.cards;

namespace TuckerMod.tuckerArtifacts.Duos;

public class AlwaysBeCountering : Artifact, IRegisterableArtifact
{
	private static Spr SpriteA; 
	private static Spr SpriteB;
	private static Spr SpriteC;
	private static Spr SpriteNone;
	public enum Mode {
		A, B, C, None
	}

	public Mode mode = Mode.None;

	public static void Register(IModHelper helper)
	{
		if (Main.Instance.DuoArtifactsApi == null || Main.Instance.JohnsonApi == null) return;

		SpriteNone = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/icons/duos/AlwaysBeCountering.png")).Sprite;
		SpriteA = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/icons/duos/Always.png")).Sprite;
		SpriteB = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/icons/duos/Be.png")).Sprite;
		SpriteC = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/icons/duos/Countering.png")).Sprite;

		helper.Content.Artifacts.RegisterArtifact("AlwaysBeCountering", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Main.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = SpriteNone,
			Name = Main.Instance.AnyLocalizations.Bind(["artifact", "duo", "AlwaysBeCountering", "name"]).Localize,
			Description = Main.Instance.AnyLocalizations.Bind(["artifact", "duo", "AlwaysBeCountering", "description"]).Localize
		});
		Main.Instance.DuoArtifactsApi.RegisterDuoArtifact<AlwaysBeCountering>([Main.Instance.JohnsonApi.JohnsonDeck.Deck, Main.Instance.TuckerDeck.Deck]);
	}

	public override Spr GetSprite() => mode switch {
		Mode.A => SpriteA,
		Mode.B => SpriteB,
		Mode.C => SpriteC,
		_ => SpriteNone
	};

	public override void OnCombatStart(State state, Combat combat) {
		mode = Mode.None;
	}

	public override void OnCombatEnd(State state) {
		mode = Mode.None;
	}

	public override void OnTurnEnd(State state, Combat combat) {
		mode = Mode.None;
	}

	public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
	{
		if (mode == Mode.B) {
			if (card.upgrade == Upgrade.B) combat.Queue(new AAddCard {
				card = new Counterattack(),
				amount = 1,
				destination = CardDestination.Hand,
				artifactPulse = Key()
			});
		}
		mode = card.upgrade switch {
			Upgrade.A => Mode.B,
			Upgrade.B => Mode.C,
			_ => Mode.A
		};
	}


	public override List<Tooltip>? GetExtraTooltips() => [
		new TTCard {
			card = new Counterattack()
		}
	];
}