using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur.actions;
using TuckerTheSaboteur.Artifacts;

namespace TuckerMod;

[HarmonyPatch(typeof(AAttack))]
public class CommJammerController
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(AAttack.Begin))]
    public static void HarmonyPrefix_ComJammerDronePatch(AAttack __instance, G g, State s, Combat c)
    {
        if (__instance is ABluntAttack) {
            foreach (Artifact item in s.EnumerateAllArtifacts()) {
                if (item is Brick brick) brick.Pulse();
            }
        }

        if (__instance.fromDroneX == null || !__instance.targetPlayer) return;

        if (g.state.EnumerateAllArtifacts().FirstOrDefault((Artifact a) => a.GetType() == typeof(CommJammer)) is not CommJammer ownedCommJammer) return;

        __instance.damage -= 1;
        ownedCommJammer.Pulse();

        return;
    }
}
