using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur.actions;

namespace TuckerTheSaboteur
{
    [HarmonyPatch(typeof(Ship))]
    public static class BufferController
    {

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Ship.OnAfterTurn))]
        public static void HarmonyPostfix_Ship_OnAfterTurn(Ship __instance, State s, Combat c)
        {
            if (__instance.Get((Status)MainManifest.statuses["buffer"].Id) > 0)
            {
                c.QueueImmediate(new AApplyBuffer
                {
                    targetPlayer = __instance.isPlayerShip
                });
            }
        }
    }
}
