using HarmonyLib;
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace TuckerTheSaboteur.actions;

[HarmonyPatch]
public class ABluntAttack : AAttack
{
    internal static bool blockEffects = false;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Ship), nameof(Ship.Get))]
    public static void HarmonyPostfix_BluntAttackBlockStunCharge(ref int __result, Status name)
    {
        if (!blockEffects) return;
        if (name != Status.stunCharge) return;
        __result = 0;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Ship), nameof(Ship.ModifyDamageDueToParts))]
    private static bool Ship_ModifyDamageDueToParts_Prefix(State s, Combat c, int incomingDamage, Part part, bool piercing = false) {
        return !blockEffects;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(AAttack), nameof(AAttack.Begin))]
    private static IEnumerable<CodeInstruction> AAttack_Begin_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase originalMethod) {
        return new SequenceBlockMatcher<CodeInstruction>(instructions)
            .Find(
                ILMatches.Ldfld("bubbleShield")
            )
            .EncompassUntil(SequenceMatcherPastBoundsDirection.Before, [
                ILMatches.Ldloc<RaycastResult>(originalMethod).CreateLdlocInstruction(out var ldLoc)
            ])
            .EncompassUntil(SequenceMatcherPastBoundsDirection.Before, [
                ILMatches.Ldarg(3).Anchor(out var anchor)
            ])
            .EncompassUntil(SequenceMatcherPastBoundsDirection.After, [
                ILMatches.Br.GetBranchTarget(out var branch)
            ])
            .Anchors()
            .PointerMatcher(anchor)
            .Insert(SequenceMatcherPastBoundsDirection.After, SequenceMatcherInsertionResultingBounds.IncludingInsertion, new List<CodeInstruction> {
                ldLoc,
                new(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType, nameof(BlockEffects))),
                new(OpCodes.Brtrue, branch.Value),
                new(OpCodes.Ldarg_3)
            })
            .AllElements();
    }

    private static bool BlockEffects(Combat c, RaycastResult result) {
        return c.stuff[result.worldX].bubbleShield && blockEffects;
    }

    private int? CopypastedGetFromX(State s, Combat c)
    {
        if (fromX.HasValue)
        {
            return fromX;
        }
        int num = (targetPlayer ? c.otherShip : s.ship).parts.FindIndex((Part p) => p.type == PType.cannon && p.active);
        if (num != -1)
        {
            return num;
        }
        return null;
    }

    public override void Begin(G g, State s, Combat c)
    {
        Ship ship = targetPlayer ? s.ship : c.otherShip;
        if (ship.Get(Status.shield) > 0 || ship.Get(Status.tempShield) > 0) {
            blockEffects = true;
            var lameAttack = new AAttack {
                damage = 0,
                targetPlayer = targetPlayer,
                fast = fast
            };
            if (Main.Instance.Helper.ModData.TryGetModData(this, OffsetAttackController.key, out int offset)) {
                lameAttack.ApplyOffset(offset);
            }
            lameAttack.Begin(g, s, c);
        } else {
            base.Begin(g, s, c);
        }
        blockEffects = false;
    }

    private static Spr GetSpr() => Main.Instance.BluntAttackSprite;
    private static Spr GetSprFail() => Main.Instance.BluntAttackFailSprite;

    public override Icon? GetIcon(State s) => new Icon(DoWeHaveCannonsThough(s) ? GetSpr() : GetSprFail(), damage, Colors.redd);

    public override List<Tooltip> GetTooltips(State s) => [
        .. base.GetTooltips(s),
        new CustomTTGlossary(
            CustomTTGlossary.GlossaryType.action,
            () => GetSpr(),
            () => Main.Instance.Localizations.Localize(["action", "bluntAttack", "name"]),
            () => Main.Instance.Localizations.Localize(["action", "bluntAttack", "description"]),
            key: "action.bluntAttack"
        )
    ];
}
