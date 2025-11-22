using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Nickel;
using TuckerTheSaboteur;

namespace TuckerMod.tuckerArtifacts.Duos;

[HarmonyPatch]
public class Cryostasis : Artifact, IRegisterableArtifact
{
	public static void Register(IModHelper helper)
	{
		if (Main.Instance.DuoArtifactsApi == null) return;

		helper.Content.Artifacts.RegisterArtifact("Cryostasis", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Main.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/icons/duos/Cryostasis.png")).Sprite,
			Name = Main.Instance.AnyLocalizations.Bind(["artifact", "duo", "Cryostasis", "name"]).Localize,
			Description = Main.Instance.AnyLocalizations.Bind(["artifact", "duo", "Cryostasis", "description"]).Localize
		});
		Main.Instance.DuoArtifactsApi.RegisterDuoArtifact<Cryostasis>([Deck.eunice, Main.Instance.TuckerDeck.Deck]);
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(Card), nameof(Card.GetActionsOverridden))]
	private static void Card_GetActionsOverridden_Postfix(State s, Combat c, Card __instance, List<CardAction> __result) {
		if (!s.EnumerateAllArtifacts().OfType<Cryostasis>().Any()) return;
		if (s.ship.Get(Status.engineStall) == 0 && s.ship.Get(Status.lockdown) == 0) return;

		foreach (CardAction action in __result) {
			if (action is AStatus aStatus && aStatus.status == Status.heat && aStatus.statusAmount > 0 && aStatus.targetPlayer)
				aStatus.statusAmount--;
		}
	}

	public override void AfterPlayerStatusAction(State state, Combat combat, Status status, AStatusMode mode, int statusAmount)
	{
		if ((status != Status.lockdown && status != Status.engineStall) || statusAmount <= 0 || state.ship.Get(status) - statusAmount > 0) return;
		Pulse();
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		.. StatusMeta.GetTooltips(Status.lockdown, 1),
		.. StatusMeta.GetTooltips(Status.engineStall, 1),
		.. StatusMeta.GetTooltips(Status.heat, DB.fakeState.ship.heatTrigger),
	];
}