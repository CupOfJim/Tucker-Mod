using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Nickel;
using TuckerTheSaboteur;

namespace TuckerMod.tuckerArtifacts.Duos;

[HarmonyPatch]
public class NavyTraining : Artifact, IRegisterableArtifact
{
	public static void Register(IModHelper helper)
	{
		if (Main.Instance.DuoArtifactsApi == null || Main.Instance.BucketApi == null) return;

		helper.Content.Artifacts.RegisterArtifact("NavyTraining", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Main.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/icons/duos/NavyTraining.png")).Sprite,
			Name = Main.Instance.AnyLocalizations.Bind(["artifact", "duo", "NavyTraining", "name"]).Localize,
			Description = Main.Instance.AnyLocalizations.Bind(["artifact", "duo", "NavyTraining", "description"]).Localize
		});
		Main.Instance.DuoArtifactsApi.RegisterDuoArtifact<NavyTraining>([Main.Instance.BucketApi.BucketDeck, Main.Instance.TuckerDeck.Deck]);
	}

	public override void OnCombatStart(State state, Combat combat)
	{
		combat.Queue(new AAddCard {
			card = new TrashAnchor(),
			destination = CardDestination.Deck,
			amount = 1,
			artifactPulse = Key()
		});
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(Combat), nameof(Combat.SendCardToHand))]
	public static void Combat_SendCardToHand_Postfix(State s, Combat __instance, Card card, int? position) {
		if (!__instance.hand.Contains(card) || card is not TrashAnchor) return;

		foreach(Artifact item in s.EnumerateAllArtifacts().OfType<NavyTraining>()) {
			__instance.Queue(new AStatus {
				status = Status.tempShield,
				statusAmount = 2,
				targetPlayer = true,
				artifactPulse = item.Key()
			});
			__instance.Queue(new AStatus {
				status = Main.Instance.BufferStatus.Status,
				statusAmount = 1,
				targetPlayer = true
			});
		}
	}

	public override List<Tooltip>? GetExtraTooltips() => [
		new TTCard {
			card = new TrashAnchor()
		},
		.. StatusMeta.GetTooltips(Status.tempShield, 2),
		.. StatusMeta.GetTooltips(Main.Instance.BufferStatus.Status, 1),
	];
}