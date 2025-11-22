using System.Collections.Generic;
using System.Reflection;
using Nickel;
using TuckerTheSaboteur;

namespace TuckerMod.tuckerArtifacts.Duos;

public class WarTable : Artifact, IRegisterableArtifact
{
	const int Threshold = 8;
	public int count = 0;

	public static void Register(IModHelper helper)
	{
		if (Main.Instance.DuoArtifactsApi == null) return;

		helper.Content.Artifacts.RegisterArtifact("WarTable", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Main.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/icons/duos/WarTable.png")).Sprite,
			Name = Main.Instance.AnyLocalizations.Bind(["artifact", "duo", "WarTable", "name"]).Localize,
			Description = Main.Instance.AnyLocalizations.Bind(["artifact", "duo", "WarTable", "description"]).Localize
		});
		Main.Instance.DuoArtifactsApi.RegisterDuoArtifact<WarTable>([Deck.peri, Main.Instance.TuckerDeck.Deck]);
	}

	public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
	{
		if (deck == Main.Instance.TuckerDeck.Deck) count++;
		if (count == Threshold) combat.Queue(new AStatus {
			status = Status.tableFlip,
			statusAmount = 1,
			targetPlayer = true,
			artifactPulse = Key()
		});
	}

	public override void OnCombatStart(State state, Combat combat)
	{
		count = 0;
	}

	public override int? GetDisplayNumber(State s) => (s == DB.fakeState || count >= Threshold) ? null : count;

	public override List<Tooltip>? GetExtraTooltips() => StatusMeta.GetTooltips(Status.tableFlip, 1);
}