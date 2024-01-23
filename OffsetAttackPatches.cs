using HarmonyLib;
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
using Microsoft.Xna.Framework.Graphics;
using TuckerMod.actions;
using static System.Collections.Specialized.BitVector32;
using TuckerTheSaboteur.actions;

namespace TuckerTheSaboteur
{
    // Big thank you to RFT for helping me with this!
    [HarmonyPatch]
    public static class OffsetAttackPatches
    {
        // ============================================
        //
        // Offset Attack functionality patches (I think these just make offset attacks work with volleys)
        //
        // ============================================

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




        // ============================================
        //
        // CANNON RECOIL ANIMATION PATCH
        //
        // ============================================




        // the firingCannons variable prevents offset attacks that land on another cannon from erasing eachother's effects
        // eg, if you're piloting the Ares and you've got both cannons active, an attack with a left offset of 2 would look like this:
        // 1  2
        // |  |
        // |  |
        // |  |  
        //    ^##^
        //   
        // without firingCannons preventing interference, what would happen is that attack 1 would trigger cannon 1 to recoil,
        // and then attack 2 would clear the recoil on cannon 1 (since attack 2 did not actually come from that part)
        // firingCannons says "hey! don't clear the recoil from this part! A previous attack decided it SHOULD recoil"
        private static int cannonAnimXOffset = 0;
        private static HashSet<int> firingCannons = new();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AAttack), nameof(AAttack.Begin))]
        public static void OffsetAttacks_CannonRecoilAnimation(AAttack __instance, G g, State s, Combat c)
        {
            // only the player gets offset attacks
            if (__instance.targetPlayer) return;

            // check to see if this is a regular, non-offset attack, and if so, clear offset attack data
            // note: volley attacks will always have a fromX value
            if (!__instance.fromX.HasValue)
            {
                cannonAnimXOffset = 0;
                return;
            }

            // if we're not a multiCannonVolley, that means we're either on a one cannon ship or we're setting up a multicannon volley
            if (!__instance.multiCannonVolley)
            {
                // extract the offset amount from the fromX
                int cannonX = s.ship.parts.FindIndex((Part p) => p.type == PType.cannon && p.active);
                cannonAnimXOffset = (__instance.fromX ?? cannonX) - cannonX;

                // if this will be a volley, 
                bool willBeVolley = (!__instance.targetPlayer && !__instance.fromDroneX.HasValue && g.state.ship.GetPartTypeCount(PType.cannon) > 1 && !__instance.multiCannonVolley);
                if (willBeVolley)
                {
                    firingCannons.Clear();
                    return;
                }

                // the below chunk of code taken from the actual AAttack.Begin source code
                // our goal here is to clear the recoil from the part the game thinks this attack is fired from
                int? num = AAttack_GetFromX(__instance, s, c);
                if (!num.HasValue) return;

                Part partAtLocalX = s.ship.GetPartAtLocalX(num.Value);
                if (partAtLocalX != null)
                {
                    partAtLocalX.pulse = 0.0; // Part.pulse is what handles the recoil animation
                }

                // and now we're begining the recoil animation on the part that ACTUALLY fired this attack
                int realPartX = num.Value - cannonAnimXOffset;
                Part partAtRealLocalX = s.ship.GetPartAtLocalX(realPartX);
                if (partAtRealLocalX != null)
                {
                    partAtRealLocalX.pulse = 1.0;
                }

            }
            else
            {
                // NOTE: a volley attack will always happen sometime after the non-volley attack that set it up
                // same as the end of the non-volley case
                int? num = AAttack_GetFromX(__instance, s, c);
                if (num.HasValue && !firingCannons.Contains(num.Value))
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



        // ============================================
        //
        // ATTACK INTENT LINES PATCH
        //
        // ============================================

        // Patch AAttack.GetTooltips to store fromX-cannonX in a global variable
        // then in prefix Combat.DrawIntentLinesForPart, 
        // if (!isplayership) return true;
        // if (part.hilight && part.type == PType.cannon) {
        //    draw intent line at i-offset
        //    return false;
        // }
        // return true;

        public static int intentXOffset = 0;
        //public static Dictionary<int, Part> columnToCannonPart = new();
        public static Dictionary<Part, List<int>> cannonFireLanes = new();
        public static bool SideEffectsOverride = false;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(AAttack), nameof(AAttack.GetTooltips))]
        public static void IntentLinesSetup(AAttack __instance, State s)
        {
            if (SideEffectsOverride) return;
            if (s.route is not Combat c) return;
            //if (c.hoveringDeck != (Deck)MainManifest.deck.Id) return;

            int cannonX = s.ship.parts.FindIndex((Part p) => p.type == PType.cannon && p.active);
            intentXOffset = (__instance.fromX ?? cannonX) - cannonX;

            for (int i = 0; i < s.ship.parts.Count; i++)
            {
                var part = s.ship.parts[i];
                if (part.type == PType.cannon && part.active) // && part.hilight)
                {
                    //columnToCannonPart[i + intentXOffset] = part;
                    if (!cannonFireLanes.ContainsKey(part)) cannonFireLanes[part] = new();
                    cannonFireLanes[part].Add(i + intentXOffset);
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Combat), nameof(Combat.DrawIntentLinesForPart))]
        public static bool IntentLinesFollowthrough(Combat __instance, Ship shipSource, Ship shipTarget, int i, Part part, Vec v)
        {
            if (!shipSource.isPlayerShip) return true;
            //if (columnToCannonPart.ContainsKey(i))
            //{
            //    part = columnToCannonPart[i];
            //}

            //if (part.type == PType.cannon && part.active && part.hilight)
            //{
            //    i += intentXOffset;
            //}

            // return true;

            if (!cannonFireLanes.ContainsKey(part)) return true;

            cannonFireLanes[part].ForEach(j => RIPPED_DrawIntentLinesForPart(__instance.stuff, shipSource, shipTarget, j, part, v));
            cannonFireLanes[part].Clear();

            return false;
        }

        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(G), nameof(G.Render))]
        //public static void IntentLinesCleanup(double deltaTime)
        //{
        //    columnToCannonPart.Clear();
        //}

        private static void RIPPED_DrawIntentLinesForPart(Dictionary<int, StuffBase> stuff, Ship shipSource, Ship shipTarget, int i, Part part, Vec v)
        {
            if (part.intent is IntentAttack || part.hilight)
            {
                Color value = ((part.intent is IntentAttack intentAttack && intentAttack.status.HasValue) ? Colors.attackStatusHint : Colors.attackHint);
                if (shipSource.isPlayerShip)
                {
                    value = Colors.attackStatusHintPlayer;
                }
                bool flag = shipTarget.HasNonEmptyPartAtWorldX(shipSource.x + i);
                Vec vec = new Vec(shipSource.xLerped * 16.0) + FxPositions.Hull(i, shipSource.isPlayerShip) - new Vec(7.0);
                Vec vec2 = new Vec(shipSource.xLerped * 16.0) + (flag ? FxPositions.Hull(i, shipTarget.isPlayerShip) : FxPositions.WayBack(i, shipTarget.isPlayerShip)) + new Vec(8.0);
                vec = vec.round();
                vec2 = vec2.round();
                if (stuff.TryGetValue(shipSource.x + i, out StuffBase value2) && (!(value2 is Missile) || shipSource.isPlayerShip))
                {
                    vec2.y = FxPositions.Drone(i).y - 2.0;
                    Spr? id = Spr.parts_hilight_endcap;
                    double x = v.x + vec.x;
                    double y = v.y + vec2.y;
                    bool flipY = !shipSource.isPlayerShip;
                    Vec? originRel = new Vec(0.0, 0.5);
                    Color? color = value;
                    BlendState screen = BlendMode.Screen;
                    Draw.Sprite(id, x, y, flipX: false, flipY, 0.0, null, originRel, null, null, color, screen);
                }
                Rect rect = Rect.FromPoints(vec, vec2);
                Draw.Rect(v.x + rect.x, v.y + rect.y, rect.w, rect.h, value, BlendMode.Screen);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Card), nameof(Card.GetDataWithOverrides))]
        public static void CardIsFlippableIfOffsetAttackIsPresent(Card __instance, ref CardData __result, State state)
        {
            if (state.route is Combat combat && !__result.flippable && state.ship.Get(Enum.Parse<Status>("tableFlip")) > 0)
            {
                foreach (CardAction action in __instance.GetActions(state, combat))
                {
                    if ((action is AAttack aAttack && aAttack.fromX.HasValue) || (action is ANoIconWrapper aniw && aniw.action is AAttack aAttack1 && aAttack1.fromX.HasValue))
                    {
                        __result.flippable = true;
                        return;
                    }
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Card), nameof(Card.GetActionsOverridden))]
        public static void EnforceFlipOnOffsetAttacks(Card __instance, List<CardAction> __result, State s, Combat c)
        {
            if (__instance.flipped)
            {
                int cannonX = s.ship.parts.FindIndex((Part p) => p.type == PType.cannon && p.active);
                foreach (CardAction item in __result)
                {
                    if (item is AAttack aAttack && aAttack.fromX.HasValue)
                    {
                        int offset = aAttack.fromX.Value - cannonX;
                        aAttack.fromX = cannonX - offset;
                    }
                    else if (item is ANoIconWrapper aniw && aniw.action is AAttack aAttack1 && aAttack1.fromX.HasValue)
                    {
                        int offset1 = aAttack1.fromX.Value - cannonX;
                        aAttack1.fromX = cannonX - offset1;
                        aAttack1.moveEnemy *= -1; // normally handled by basegame, but the wrapper gets in the way
                    }
                }
                foreach (CardAction item in __result)
                { 
                    if (item is ATooltipDummy atd)
                    {
                        atd.UpdateFromStandin(s);
                    }
                }
            }
        }
    }
}
