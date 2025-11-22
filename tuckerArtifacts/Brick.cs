using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Nickel;
using TuckerTheSaboteur.actions;

namespace TuckerTheSaboteur.Artifacts;

[HarmonyPatch(typeof(Card))]
public class Brick : Artifact, IRegisterableArtifact {
	public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("Brick", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Main.Instance.TuckerDeck.Deck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/icons/Brick.png")).Sprite,
			Name = Main.Instance.AnyLocalizations.Bind(["artifact", "Brick", "name"]).Localize,
			Description = Main.Instance.AnyLocalizations.Bind(["artifact", "Brick", "description"]).Localize
		});
	}

	[HarmonyPostfix]
	[HarmonyPatch(nameof(Card.GetActionsOverridden))]
	private static void ApplyBrick(ref List<CardAction> __result, Card __instance, State s, Combat c) {
		foreach (Artifact item in s.EnumerateAllArtifacts()) {
			if (item is Brick) {
				foreach (CardAction action in __result) {
					if (action is ABluntAttack bluntAttack && !bluntAttack.targetPlayer) bluntAttack.damage++; 
				}
			}
		}
	}

	public override List<Tooltip>? GetExtraTooltips() => new ABluntAttack {
		damage = 1
	}.GetTooltips(DB.fakeState);
}