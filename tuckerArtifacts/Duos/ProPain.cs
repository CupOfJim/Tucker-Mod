using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Nickel;
using TuckerTheSaboteur;
using TuckerTheSaboteur.actions;
using TuckerTheSaboteur.cards;

namespace TuckerMod.tuckerArtifacts.Duos;

public class ProPain : Artifact, IRegisterableArtifact
{
	private static AAttack? AttackContext;

	public static void Register(IModHelper helper)
	{
		if (Main.Instance.DuoArtifactsApi == null || Main.Instance.MarielleApi == null) return;

		Main.Instance.Harmony.TryPatch(
			logger: Main.Instance.Logger,
			original: AccessTools.DeclaredMethod(typeof(Ship), nameof(Ship.NormalDamage)),
			postfix: AccessTools.DeclaredMethod(typeof(ProPain), nameof(Ship_NormalDamage_Postfix))
		);
		Main.Instance.Harmony.TryPatch(
			logger: Main.Instance.Logger,
			original: AccessTools.DeclaredMethod(typeof(AAttack), nameof(AAttack.Begin)),
			prefix: AccessTools.DeclaredMethod(typeof(ProPain), nameof(AAttack_Begin_Prefix)),
			finalizer: AccessTools.DeclaredMethod(typeof(ProPain), nameof(AAttack_Begin_Finalizer))
		);

		helper.Content.Artifacts.RegisterArtifact("ShieldBash", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Main.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/icons/duos/ProPain.png")).Sprite,
			Name = Main.Instance.AnyLocalizations.Bind(["artifact", "duo", "ProPain", "name"]).Localize,
			Description = Main.Instance.AnyLocalizations.Bind(["artifact", "duo", "ProPain", "description"]).Localize
		});
		Main.Instance.DuoArtifactsApi.RegisterDuoArtifact<ProPain>([Main.Instance.MarielleApi.MarielleDeck().Deck, Main.Instance.TuckerDeck.Deck]);
	}

	private static void Ship_NormalDamage_Postfix(State s, Combat c) {
		if (AttackContext == null || AttackContext.targetPlayer) return;

		var artifact = s.EnumerateAllArtifacts().OfType<ProPain>().FirstOrDefault();
		if (artifact == null) return;

		if (AttackContext is ABluntAttack) c.QueueImmediate(new AStatus {
			status = Status.heat,
			statusAmount = 2,
			targetPlayer = false
		});
		if (AttackContext.piercing) c.QueueImmediate(new AStatus {
			status = Status.heat,
			statusAmount = 1,
			targetPlayer = false
		});
	}

	private static void AAttack_Begin_Prefix(AAttack __instance)
		=> AttackContext = __instance;

	private static void AAttack_Begin_Finalizer()
		=> AttackContext = null;


	public override List<Tooltip>? GetExtraTooltips() => [
		new TTGlossary("action.attackPiercing"),
		.. new ABluntAttack().GetTooltips(DB.fakeState),
		.. StatusMeta.GetTooltips(Status.heat, DB.fakeState.ship.heatTrigger),
	];
}