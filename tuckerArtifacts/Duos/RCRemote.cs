using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Nickel;
using TuckerTheSaboteur;

namespace TuckerMod.tuckerArtifacts.Duos;

public class RCRemote : Artifact, IRegisterableArtifact
{
	public static void Register(IModHelper helper)
	{
		if (Main.Instance.DuoArtifactsApi == null) return;

		Main.Instance.Harmony.TryPatch(
			logger: Main.Instance.Logger,
			original: typeof(Combat).GetMethods().Where(m => m.Name.Contains("GetCardAutopilot")).First(),
			postfix: AccessTools.DeclaredMethod(typeof(RCRemote), nameof(Card_RenderCards_GetCardAutopilot_Postfix))
		);

		helper.Content.Artifacts.RegisterArtifact("RCRemote", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Main.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/icons/duos/RCRemote.png")).Sprite,
			Name = Main.Instance.AnyLocalizations.Bind(["artifact", "duo", "RCRemote", "name"]).Localize,
			Description = Main.Instance.AnyLocalizations.Bind(["artifact", "duo", "RCRemote", "description"]).Localize
		});
		Main.Instance.DuoArtifactsApi.RegisterDuoArtifact<RCRemote>([Deck.hacker, Main.Instance.TuckerDeck.Deck]);
	}

	private static void Card_RenderCards_GetCardAutopilot_Postfix(State s, Card card, List<Card> hand, ref int? __result) {
		if (__result.HasValue || !s.EnumerateAllArtifacts().OfType<RCRemote>().Any()) return;

		if (hand.Count > 1 && (hand[0] == card || hand[^1] == card)) {
			if (s.route is Combat c) {
				int autopilot = c.otherShip.Get(Status.autopilot);
				__result = autopilot == 0 ? null : (hand[0] == card ? -autopilot : autopilot);
				return;
			}
		}
	}

	public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
	{
		int autopilot = combat.otherShip.Get(Status.autopilot);
		if (autopilot == 0 || handCount == 1) return;
		
		if (handPosition == 0) combat.Queue(new AMove {
			dir = -autopilot,
			targetPlayer = false
		});
		if (handPosition == handCount-1) combat.Queue(new AMove {
			dir = autopilot,
			targetPlayer = false
		});
	}

	public override void AfterPlayerStatusAction(State state, Combat combat, Status status, AStatusMode mode, int statusAmount)
	{
		if ((status == Status.lockdown || status == Status.engineStall) && statusAmount > 0) combat.Queue(new AStatus {
			status = Status.autopilot,
			statusAmount = 1,
			targetPlayer = false,
			artifactPulse = Key()
		});
	}


	public override List<Tooltip>? GetExtraTooltips() => [
		.. StatusMeta.GetTooltips(Status.lockdown, 1),
		.. StatusMeta.GetTooltips(Status.engineStall, 1),
		.. StatusMeta.GetTooltips(Status.autopilot, 1)
	];
}