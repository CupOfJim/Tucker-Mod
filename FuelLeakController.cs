using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur.actions;

namespace TuckerTheSaboteur
{
    [HarmonyPatch(typeof(AMove))]
    public static class FuelLeakController
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(AMove.Begin))]
        public static void HarmonyPostfix_AMove_Begin(AMove __instance, G g, State s, Combat c)
        {
            Ship ship = (__instance.targetPlayer ? s.ship : c.otherShip);
            if (ship.Get((Status)MainManifest.statuses["fuel_leak"].Id) > 0)
            {
                c.QueueImmediate(new AFuelLeakDamage
                {
                    targetPlayer = __instance.targetPlayer
                });
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(Ship), nameof(Ship.OnBeginTurn))]
        public static void DecrementFuelLeak(Ship __instance, State s, Combat c)
        {
            var fuelLeak = __instance.Get((Status)MainManifest.statuses["fuel_leak"].Id);
            if (fuelLeak > 0)
            {
                __instance.Set((Status)MainManifest.statuses["fuel_leak"].Id, fuelLeak - 1);
            }
        }
    }
}
