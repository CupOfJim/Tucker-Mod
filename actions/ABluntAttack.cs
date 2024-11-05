using FMOD;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur.Artifacts;

namespace TuckerTheSaboteur.actions;

[HarmonyPatch]
public class ABluntAttack : AAttack
{
    static bool blockEffects = false;

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
            damage = 0;
            status = null;
            stunEnemy = false;
            moveEnemy = 0;
            weaken = false;
            brittle = false;
            armorize = false;
        }
        blockEffects = true;
        base.Begin(g, s, c);
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
