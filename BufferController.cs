using HarmonyLib;

namespace TuckerTheSaboteur;

[HarmonyPatch(typeof(Ship))]
public static class BufferController
{

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Ship.OnBeginTurn))]
    public static void HarmonyPostfix_Ship_OnAfterTurn(Ship __instance, State s, Combat c)
    {
        int leak = __instance.Get(Main.Instance.BufferStatus.Status);
        if (leak > 0)
        {

            c.QueueImmediate(new AStatus {
                status = Status.tempShield,
                targetPlayer = __instance.isPlayerShip,
                statusAmount = leak,
                statusPulse = Main.Instance.BufferStatus.Status
            });
            if (__instance.Get(Status.timeStop) == 0) {
                __instance.Add(Main.Instance.BufferStatus.Status, -1);
            }
        }
    }
}