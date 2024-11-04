using System.Reflection;
using HarmonyLib;
using Nickel;
using TuckerTheSaboteur.cards;

namespace TuckerTheSaboteur.Artifacts;

[HarmonyPatch]
public class ArtofWar : Artifact, IRegisterableArtifact
{

    public int lastShield = 0;
	public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("ArtofWar", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Main.Instance.TuckerDeck.Deck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/icons/Art_of_War.png")).Sprite,
			Name = Main.Instance.AnyLocalizations.Bind(["artifact", "ArtofWar", "name"]).Localize,
			Description = Main.Instance.AnyLocalizations.Bind(["artifact", "ArtofWar", "description"]).Localize
		});
	}

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Ship), nameof(Ship.NormalDamage))]
    private static void DidShieldPop(Ship __instance, DamageDone __result, State s, Combat c, int incomingDamage, int? maybeWorldGridX, bool piercing) {
        if (__result.poppedShield) {
            c.QueueImmediate(new AAddCard {
                card = new Counterattack(),
                destination = CardDestination.Hand
            });
        }
    }
}