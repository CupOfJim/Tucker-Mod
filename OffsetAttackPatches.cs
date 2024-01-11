﻿using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;

namespace TuckerTheSaboteur
{
    // Big thank you to RFT for helping me with this!
    [HarmonyPatch]
    public static class OffsetAttackPatches
    {
        public static int xOffset = 0;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AVolleyAttackFromAllCannons), nameof(AVolleyAttackFromAllCannons.Begin))]
        public static bool HarmonyPrefix_AVolleyAttackFromAllCannons_Begin(AVolleyAttackFromAllCannons __instance, G g, State s, Combat c)
        {
            xOffset = 0;
            if (__instance.attack.fromX != null)
            {
                int baseX = (__instance.attack.targetPlayer ? c.otherShip : s.ship).parts.FindIndex((Part p) => p.type == PType.cannon && p.active);
                xOffset = (int)(__instance.attack.fromX - baseX);
            }

            return true;
        }

        public static int OffsetMyX(int myx) { return myx + xOffset; }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(AVolleyAttackFromAllCannons), nameof(AVolleyAttackFromAllCannons.Begin))]
        private static IEnumerable<CodeInstruction> HarmonyTranspiler_AVolleyAttackFromAllCannons_Begin(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
        {
            try
            {
                var localVars = originalMethod.GetMethodBody()!.LocalVariables;

                return new SequenceBlockMatcher<CodeInstruction>(instructions)
                    .Find(
                        ILMatches.Ldarg(0),
                        ILMatches.Ldfld("attack"),
                        ILMatches.Ldloc<int>(localVars)
                    )
                    .Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.JustInsertion, new List<CodeInstruction>()
                    {
                        new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(typeof(OffsetAttackPatches), nameof(OffsetAttackPatches.OffsetMyX)))
                    })
                    .AllElements();
            }
            catch (Exception e)
            {
                Console.WriteLine("AVolleyAttackFromAllCannons.Begin transpilation patch failed!");
                Console.WriteLine(e);
                return instructions;
            }
        }

        // that above prefix and transpile does this:
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(AVolleyAttackFromAllCannons), nameof(AVolleyAttackFromAllCannons.Begin))]
        //public static bool Begin(AVolleyAttackFromAllCannons __instance, G g, State s, Combat c)
        //{
        //    __instance.timer = 0.0;
        //    __instance.attack.multiCannonVolley = true;
        //    __instance.attack.fast = true;
        //    List<AAttack> list = new List<AAttack>();

        //    int xOffset = 0;
        //    if (__instance.attack.fromX != null)
        //    {
        //        int baseX = (__instance.attack.targetPlayer ? c.otherShip : s.ship).parts.FindIndex((Part p) => p.type == PType.cannon && p.active);
        //        xOffset = (int)(__instance.attack.fromX - baseX);
        //    }

        //    int num = 0;
        //    foreach (Part part in s.ship.parts)
        //    {
        //        if (part.type == PType.cannon && part.active)
        //        {
        //            __instance.attack.fromX = num + xOffset;
        //            list.Add(Mutil.DeepCopy(__instance.attack));
        //        }
        //        num++;
        //    }
        //    c.QueueImmediate(list);

        //    return false;
        //}



        private static int cannonAnimXOffset = 0;
        private static HashSet<int> firingCannons = new();

        //// EffectSpawner.Cannon(g, targetPlayer, raycastResult, dmg, isBeam);
        //// prefix patch it to do nothing

        //// aattack begin prefix: if not is volley and xoffset, set cannonAnimXOffset = xoffset (otherwise set it to 0)
        //// effectspawner.cannon prefix- raycastResult.x -= cannonAnimXOffset
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AAttack), nameof(AAttack.Begin))]
        public static void CannonRecoilAnimationSetup(AAttack __instance, G g, State s, Combat c)
        {
            if (__instance.targetPlayer) return;
            
            if (!__instance.multiCannonVolley)
            {
                cannonAnimXOffset = 0;
                if (!__instance.fromX.HasValue) return;

                int cannonX = s.ship.parts.FindIndex((Part p) => p.type == PType.cannon && p.active);
                cannonAnimXOffset = (__instance.fromX ?? 0) - cannonX;

                bool willBeVolley = (!__instance.targetPlayer && !__instance.fromDroneX.HasValue && g.state.ship.GetPartTypeCount(PType.cannon) > 1 && !__instance.multiCannonVolley);
                if (willBeVolley)
                {
                    firingCannons.Clear();
                    return;
                }

                int? num = AAttack_GetFromX(__instance, s, c);
                if (!num.HasValue) return;

                Part partAtLocalX = s.ship.GetPartAtLocalX(num.Value);
                if (partAtLocalX != null)
                {
                    partAtLocalX.pulse = 0.0;
                }

                int realPartX = num.Value - cannonAnimXOffset;
                Part partAtRealLocalX = s.ship.GetPartAtLocalX(realPartX);
                if (partAtRealLocalX != null)
                {
                    partAtRealLocalX.pulse = 1.0;
                }

            }
            else
            {

                int? num = AAttack_GetFromX(__instance, s, c);
                if (!firingCannons.Contains(num.Value))
                {
                    Part partAtLocalX = s.ship.GetPartAtLocalX(num.Value);
                    if (partAtLocalX != null)
                    {
                        partAtLocalX.pulse = 0.0;
                    }
                }

                int realPartX = num.Value - cannonAnimXOffset;
                Part partAtRealLocalX = s.ship.GetPartAtLocalX(realPartX);
                if (partAtRealLocalX != null)
                {
                    partAtRealLocalX.pulse = 1.0;
                }
                firingCannons.Add(realPartX);

            }
        }

        private static int? AAttack_GetFromX(AAttack a, State s, Combat c)
        {
            if (a.fromX.HasValue)
            {
                return a.fromX;
            }
            int num = (a.targetPlayer ? c.otherShip : s.ship).parts.FindIndex((Part p) => p.type == PType.cannon && p.active);
            if (num != -1)
            {
                return num;
            }
            return null;
        }

    }
}
