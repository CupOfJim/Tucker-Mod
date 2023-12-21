using HarmonyLib;
using Nanoray.Shrike.Harmony;
using Nanoray.Shrike;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
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

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(Card), nameof(Card.RenderAction))]
        private static IEnumerable<CodeInstruction> HarmonyTranspile_Card_RenderAction(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod)
        {
            try
            {
                var localVars = originalMethod.GetMethodBody()!.LocalVariables;

                return new SequenceBlockMatcher<CodeInstruction>(instructions)
                    .Find(ILMatches.)
                    .Find(
                        ILMatches.Ldloca<Icon>(localVars),
                        ILMatches.Call("get_HasValue"),
                        ILMatches.Brfalse
                    )
                    .PointerMatcher(SequenceMatcherRelativeElement.First)
                    .CreateLdlocInstruction(out var action)
                    .Advance(2)
                    .Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.JustInsertion,
                        new List<CodeInstruction>
                        {
                        new(OpCodes.Ldarg_0),
                        action,
                        new(OpCodes.Call, AccessTools.DeclaredMethod(typeof(OffsetAttackPatches), nameof(DrawThing)))
                        })
                    .AllElements();
            }
            catch (Exception e)
            {
                Console.WriteLine("Card.RenderAction patch failed!");
                Console.WriteLine(e);
                return instructions;
            }
        }


        private static void DrawThing(Card c, CardAction a)
        {
            if (a is AAttack aattack && aattack.fromX != null)
            {
                int baseX = (__instance.attack.targetPlayer ? c.otherShip : s.ship).parts.FindIndex((Part p) => p.type == PType.cannon && p.active);
                int xOffset = (int)(aattack.fromX - baseX); IconAndOrNumber((Spr)TuckerTheSaboteur.MainManifest.sprites["icons/offset_attack"].id, xOffset, Colors.redd);
            }
        }

        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(AJupiterShoot), nameof(AJupiterShoot.Begin))]
        //public static bool Begin(AJupiterShoot __instance, G g, State s, Combat c)
        //{
        //    __instance.timer = 0.0;
        //    SortedList<int, CardAction> sortedList = new SortedList<int, CardAction>();
        //    foreach (KeyValuePair<int, StuffBase> item in c.stuff)
        //    {
        //        if (!(item.Value is JupiterDrone))
        //        {
        //            continue;
        //        }

        //        int xOffset = 0;
        //        if (__instance.attackCopy.fromX != null)
        //        {
        //            int baseX = s.ship.parts.FindIndex((Part p) => p.type == PType.cannon && p.active);
        //            xOffset = (int)(__instance.attackCopy.fromX - baseX);
        //        }

        //        AAttack aAttack = Mutil.DeepCopy(__instance.attackCopy);
        //        aAttack.fast = true;
        //        aAttack.fromX = null;
        //        aAttack.fromDroneX = item.Value.x + xOffset;
        //        aAttack.targetPlayer = item.Value.targetPlayer;
        //        aAttack.shardcost = 0;
        //        int damage = aAttack.damage;
        //        foreach (Artifact item2 in s.EnumerateAllArtifacts())
        //        {
        //            aAttack.damage += item2.ModifyBaseJupiterDroneDamage(s, c, item.Value);
        //            if (aAttack.damage > damage)
        //            {
        //                aAttack.artifactPulse = item2.Key();
        //                damage = aAttack.damage;
        //            }
        //        }
        //        sortedList.Add(item.Value.x + xOffset, aAttack);
        //    }
        //    c.QueueImmediate(sortedList.Values);

        //    return false;
        //}
    }
}
