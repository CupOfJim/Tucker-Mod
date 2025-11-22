using HarmonyLib;
using System;
using TuckerTheSaboteur.actions;

namespace TuckerTheSaboteur;

[HarmonyPatch]
public static class FuelLeakController
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(AMove), nameof(AMove.Begin))]
    public static void HarmonyPrefix_AMove_Begin(AMove __instance, G g, State s, Combat c, ref int __state)
    {
        Ship ship = __instance.targetPlayer ? s.ship : c.otherShip;
        __state = ship.x;
    }
    [HarmonyPostfix]
    [HarmonyPatch(typeof(AMove), nameof(AMove.Begin))]
    public static void HarmonyPostfix_AMove_Begin(AMove __instance, G g, State s, Combat c, ref int __state)
    {
        Ship ship = __instance.targetPlayer ? s.ship : c.otherShip;
        int leak = ship.Get(Main.Instance.FuelLeakStatus.Status);
        int diff = Math.Abs(ship.x - __state);
        if (leak > 0 && diff > 0)
        {
            c.QueueImmediate(new AFuelLeakDamage
            {
                targetPlayer = __instance.targetPlayer,
                amount = diff * leak
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