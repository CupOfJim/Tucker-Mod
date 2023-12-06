using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuckerTheSaboteur
{
    [HarmonyPatch(typeof(AMove))]
    public static class FuelLeakController
    {

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Combat.SendCardToExhaust))]
        public static void HarmonyPostfix_AMove_Begin(AMove __instance, G g, State s, Combat c)
        {
            Ship ship = (__instance.targetPlayer ? s.ship : c.otherShip);
            if (ship.Get((Status)MainManifest.statuses["fuel_leak"].Id) > 0)
            {
                c.QueueImmediate(new ACorrodeDamage
                {
                    targetPlayer = __instance.targetPlayer
                });
            }
        }
    }
}
