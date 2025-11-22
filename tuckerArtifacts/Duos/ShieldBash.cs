using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Nickel;
using TuckerTheSaboteur;
using TuckerTheSaboteur.actions;
using TuckerTheSaboteur.cards;

namespace TuckerMod.tuckerArtifacts.Duos;

public class ShieldBash : Artifact, IRegisterableArtifact
{
	public static void Register(IModHelper helper)
	{
		if (Main.Instance.DuoArtifactsApi == null || Main.Instance.DestinyApi == null) return;

		Main.Instance.Harmony.TryPatch(
			logger: Main.Instance.Logger,
			original: AccessTools.DeclaredMethod(typeof(Card), nameof(Card.GetActionsOverridden)),
			postfix: AccessTools.DeclaredMethod(typeof(ShieldBash), nameof(ApplyShieldBash))
		);

		helper.Content.Artifacts.RegisterArtifact("ShieldBash", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Main.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/icons/duos/ShieldBash.png")).Sprite,
			Name = Main.Instance.AnyLocalizations.Bind(["artifact", "duo", "ShieldBash", "name"]).Localize,
			Description = Main.Instance.AnyLocalizations.Bind(["artifact", "duo", "ShieldBash", "description"]).Localize
		});
		Main.Instance.DuoArtifactsApi.RegisterDuoArtifact<ShieldBash>([Main.Instance.DestinyApi.DestinyDeck.Deck, Main.Instance.TuckerDeck.Deck]);
	}

	private static void ApplyShieldBash(ref List<CardAction> __result, Card __instance, State s, Combat c) {
		foreach (Artifact item in s.EnumerateAllArtifacts()) {
			if (item is ShieldBash) {
				foreach (CardAction action in __result) {
					if (action is ABluntAttack bluntAttack && !bluntAttack.targetPlayer) {
						bluntAttack.damage += s.ship.Get(Status.tempShield) / 3;
						bluntAttack.damage += s.ship.Get(Status.shield) / 2;
						bluntAttack.damage += s.ship.Get(Main.Instance.DestinyApi!.PristineShieldStatus.Status);
						bluntAttack.damage += s.ship.Get(Status.perfectShield) * 2;
					}
				}
			}
		}
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		.. new ABluntAttack().GetTooltips(DB.fakeState),
		.. StatusMeta.GetTooltips(Status.tempShield, 3),
		.. StatusMeta.GetTooltips(Status.shield, 2),
		.. StatusMeta.GetTooltips(Main.Instance.DestinyApi!.PristineShieldStatus.Status, 1),
		.. StatusMeta.GetTooltips(Status.perfectShield, 1),
	];
}