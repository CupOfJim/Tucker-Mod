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
    public class ComJammerController
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(AAttack.Begin))]
        public static bool Begin(AAttack __instance, G g, State s, Combat c)
        {
            if (__instance.fromDroneX == null || !__instance.targetPlayer) return true;

            var ownedComJammer = g.state.EnumerateAllArtifacts().Where((Artifact a) => a.GetType() == typeof(ComJammer)).FirstOrDefault() as ComJammer;
            __instance.damage -= 1;
            ownedComJammer.Pulse();

            return true;
        }
    }
}
