using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuckerTheSaboteur
{
    [HarmonyPatch]
    public static class OffsetAttackPatches
    {

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AVolleyAttackFromAllCannons), nameof(AVolleyAttackFromAllCannons.Begin))]
        public static bool Begin(AVolleyAttackFromAllCannons __instance, G g, State s, Combat c)
        {
            __instance.timer = 0.0;
            __instance.attack.multiCannonVolley = true;
            __instance.attack.fast = true;
            List<AAttack> list = new List<AAttack>();

            int xOffset = 0;
            if (__instance.attack.fromX != null)
            {
                int baseX = (__instance.attack.targetPlayer ? c.otherShip : s.ship).parts.FindIndex((Part p) => p.type == PType.cannon && p.active);
                xOffset = (int)(__instance.attack.fromX - baseX);
            }

            int num = 0;
            foreach (Part part in s.ship.parts)
            {
                if (part.type == PType.cannon && part.active)
                {
                    __instance.attack.fromX = num + xOffset;
                    list.Add(Mutil.DeepCopy(__instance.attack));
                }
                num++;
            }
            c.QueueImmediate(list);

            return false;
        }
    }
}
