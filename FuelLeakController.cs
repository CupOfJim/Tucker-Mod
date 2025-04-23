using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur.actions;

namespace TuckerTheSaboteur;

public static class FuelLeakController
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(AMove), nameof(AMove.Begin))]
    public static void HarmonyPostfix_AMove_Begin(AMove __instance, G g, State s, Combat c)
    {
        Ship ship = __instance.targetPlayer ? s.ship : c.otherShip;
        int leak = ship.Get(Main.Instance.FuelLeakStatus.Status);
        if (__instance.dir != 0 && leak > 0)
        {
            c.QueueImmediate(new AFuelLeakDamage
            {
                targetPlayer = __instance.targetPlayer,
                amount = Math.Abs(__instance.dir) * leak
            });
        }
    }


    [HarmonyPostfix]
    [HarmonyPatch(typeof(Ship), nameof(Ship.OnBeginTurn))]
    public static void DecrementFuelLeak(Ship __instance, State s, Combat c)
    {
        var fuelLeak = __instance.Get(Main.Instance.FuelLeakStatus.Status);
        if (fuelLeak > 0)
        {
            __instance.Set(Main.Instance.FuelLeakStatus.Status, fuelLeak - 1);
        }
    }
}