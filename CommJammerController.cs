using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckertheSabotuer.Artifacts;

namespace TuckerMod
{
    [HarmonyPatch(typeof(AAttack))]
    public class CommJammerController
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(AAttack.Begin))]
        public static bool HarmonyPrefix_ComJammerDronePatch(AAttack __instance, G g, State s, Combat c)
        {
            if (__instance.fromDroneX == null || !__instance.targetPlayer) return true;

            var ownedCommJammer = g.state.EnumerateAllArtifacts().Where((Artifact a) => a.GetType() == typeof(CommJammer)).FirstOrDefault() as CommJammer;
            if (ownedCommJammer == null) return true;

            __instance.damage -= 1;
            ownedCommJammer.Pulse();

            return true;
        }
    }
}
