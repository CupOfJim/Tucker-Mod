using HarmonyLib;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuckerTheSaboteur.actions
{
    [HarmonyPatch(typeof(Card))]
    public class ATooltipDummy : ADummyAction
    {
        public delegate void OnGetTooltips(State s);

        public OnGetTooltips onGetTooltips;
        public List<Tooltip> tooltips;
        public List<Icon>? icons;

        public CardAction? standinFor;

        private int stunOverride = 0;

        public void UpdateFromStandin(State s)
        {
            if (standinFor == null) return;

            ATooltipDummy newStandin = BuildStandIn(standinFor, s, stunOverride);
            onGetTooltips = newStandin.onGetTooltips;
            tooltips = newStandin.tooltips;
            icons = newStandin.icons;

            tooltips.Add(new TTText("UPDATED FROM STANDIN"));
        }

        public static List<CardAction> BuildStandinsAndWrapRealActions(List<CardAction> actions, State s)
        {
            List<CardAction> finalActions = new();
            for (int i = 0; i < actions.Count; i++) finalActions.Add(new ADummyAction());
            finalActions.AddRange(ATooltipDummy.BuildStandIns(actions, s));
            finalActions.AddRange(actions.Select(a => new TuckerMod.actions.ANoIconWrapper() { action = a }));

            return finalActions;
        }

        public static List<ATooltipDummy> BuildStandIns(List<CardAction> actions, State s)
        {
            int availableStunCharge = s.ship.Get(Enum.Parse<Status>("stunCharge"));
            return actions.Select(a => BuildStandIn(a, s, availableStunCharge--)).ToList();
        }

        public static ATooltipDummy BuildStandIn(CardAction action, State s, int availableStunCharge)
        {
            if (action is AAttack aattack)
            {
                int cannonX = s.ship.parts.FindIndex((Part p) => p.type == PType.cannon && p.active);
                return BuildFromAttack(aattack, s, cannonX: cannonX, availableStunCharge: availableStunCharge);
            }

            if (action is AStatus astatus)
            {
                return BuildFromStatus(astatus, s);
            }

            Icon? icon = action.GetIcon(s);
            return new ATooltipDummy()
            {
                tooltips = action.GetTooltips(s),
                icons = icon == null ? new() : new() { (Icon)action.GetIcon(s) },
                standinFor = action
            };
        }

        public static ATooltipDummy BuildFromStatus(AStatus astatus, State s)
        {
            List<Icon> icons = new();

            if (!astatus.targetPlayer)
            {
                icons.Add(new Icon(Enum.Parse<Spr>("icons_outgoing"), null, Colors.textMain));
            }

            Icon? icon = astatus.GetIcon(s);
            if (icon != null)
            {
                icons.Add((Icon)icon);
            }

            return new ATooltipDummy()
            {
                tooltips = astatus.GetTooltips(s),
                icons = icons,
                standinFor = astatus
            };
        }

        public static List<Tooltip> GetTooltipsNoSideEffects(AAttack aattack, State s)
        {
            var sshipparts = s.ship.parts;
            s.ship.parts = new();
            var c = s.route is Combat combat ? combat : null;
            var cstuff = c != null ? c.stuff : null;
            if (c != null) c.stuff = new();

            OffsetAttackPatches.SideEffectsOverride = true;
            var tooltips = aattack.GetTooltips(s);
            OffsetAttackPatches.SideEffectsOverride = false;

            if (c != null) c.stuff = cstuff;
            s.ship.parts = sshipparts;

            return tooltips;
        }

        public static ATooltipDummy BuildFromAttack(AAttack aattack, State s, int? cannonX = null, bool hideOutgoingArrow = true, int availableStunCharge = 0)
        {
            List<Icon> icons = new();
            var tooltips = GetTooltipsNoSideEffects(aattack, s);

            if (cannonX != null && aattack.fromX != null && aattack.fromX != cannonX)
            {
                int offset = (int)(aattack.fromX - cannonX);
                Spr spr = offset < 0
                    ? (Spr)MainManifest.sprites["icons/Offset_Shot_Left"].Id
                    : (Spr)MainManifest.sprites["icons/Offset_Shot_Right"].Id;

                icons.Add(new Icon(spr, Math.Abs(offset), Colors.redd));

                if (offset <  -1) tooltips.Add(new TTGlossary(MainManifest.glossary["ALeftShotOffset"].Head, Math.Abs(offset)));
                if (offset >   1) tooltips.Add(new TTGlossary(MainManifest.glossary["ARightShotOffset"].Head, Math.Abs(offset)));
                if (offset ==  1) tooltips.Add(new TTGlossary(MainManifest.glossary["ARightShotOffsetSingle"].Head, 1));
                if (offset == -1) tooltips.Add(new TTGlossary(MainManifest.glossary["ALeftShotOffsetSingle"].Head, 1));
            }

            icons.Add(new Icon(Enum.Parse<Spr>("icons_attack"), aattack.damage, Colors.redd));

            if (aattack.stunEnemy || availableStunCharge > 0)
            {
                icons.Add(new Icon(Enum.Parse<Spr>("icons_stun"), null, Colors.textMain));
            }

            if (aattack.status != null)
            {
                // this cast is ridiculous. It's needed because C# still thinks aattack.status can be null, even though it must be non-null to get here
                var icon = new AStatus() { 
                    status = (Status)aattack.status, 
                    targetPlayer = hideOutgoingArrow, 
                    statusAmount = aattack.statusAmount 
                }.GetIcon(s);

                if(icon != null)
                {
                    if (!hideOutgoingArrow) icons.Add(new Icon(Enum.Parse<Spr>("icons_outgoing"), null, Colors.textMain));
                    icons.Add((Icon)icon);
                }
            }

            if (aattack.moveEnemy != 0)
            {
                Spr spr = aattack.moveEnemy < 0
                    ? Enum.Parse<Spr>("icons_moveLeftEnemy")
                    : Enum.Parse<Spr>("icons_moveRightEnemy");
                icons.Add(new Icon(spr, Math.Abs(aattack.moveEnemy), Colors.redd));
            }

            return new ATooltipDummy()
            {
                disabled = aattack.disabled,
                tooltips = tooltips,
                icons = icons,
                onGetTooltips = (s) => aattack.GetTooltips(s),
                standinFor = aattack,
                stunOverride = availableStunCharge
            };
        }

        public override List<Tooltip> GetTooltips(State s)
        {
            if (onGetTooltips != null) onGetTooltips(s);
            return tooltips ?? new();
        }

        public override Icon? GetIcon(State s)
        {
            return null;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Card.RenderAction))]
        public static bool HarmonyPrefix_Card_RenderAction(ref int __result, G g, State state, CardAction action, bool dontDraw = false, int shardAvailable = 0, int stunChargeAvailable = 0, int bubbleJuiceAvailable = 0)
        {
            if (action is ATooltipDummy aTooltipDummy)
            {
                if (aTooltipDummy.icons == null)
                {
                    return true;
                }

                if (dontDraw)
                {
                    return false;
                }

                var iconNumberPadding = aTooltipDummy.icons.Count >= 3 ? 1 : 2;
                var iconPadding = aTooltipDummy.icons.Count >= 3 ? 2 : 4;

                Color spriteColor = (action.disabled ? Colors.disabledIconTint : new Color("ffffff"));
                int w = 0;
                bool isFirst = true;

                foreach (var icon in aTooltipDummy.icons)
                {
                    IconAndOrNumber(icon.path, ref isFirst, ref w, g, action, state, spriteColor, true, amount: icon.number, iconWidth: SpriteLoader.Get(icon.path).Width, iconNumberPadding: iconNumberPadding, iconPadding: iconPadding);
                }

                w = -w / 2;
                isFirst = true;
                foreach (var icon in aTooltipDummy.icons)
                {
                    IconAndOrNumber(icon.path, ref isFirst, ref w, g, action, state, spriteColor, dontDraw, amount: icon.number, iconWidth: SpriteLoader.Get(icon.path).Width, iconNumberPadding: iconNumberPadding, iconPadding: iconPadding, textColor: icon.color);
                }
            } 
            else
            {
                return true;
            }

            return false;
        }

        private static void IconAndOrNumber(Spr icon, ref bool isFirst, ref int w, G g, CardAction action, State state, Color spriteColor, bool dontDraw, int iconNumberPadding = 2, int iconWidth = 8, int numberWidth = 6, int? amount = null, Color? textColor = null, bool flipY = false, int? x = null, int iconPadding = 4)
        {
            if (!isFirst)
            {
                w += iconPadding;
            }
            if (!dontDraw)
            {
                Rect? rect = new Rect(w);
                Vec xy = g.Push(null, rect).rect.xy;
                Draw.Sprite(icon, xy.x, xy.y, flipX: false, flipY, 0.0, null, null, null, null, spriteColor);
                g.Pop();
            }
            w += iconWidth;
            if (amount.HasValue)
            {
                int valueOrDefault4 = amount.GetValueOrDefault();
                if (!x.HasValue)
                {
                    w += iconNumberPadding;
                    string text = DB.IntStringCache(valueOrDefault4);
                    if (!dontDraw)
                    {
                        Rect? rect = new Rect(w);
                        Vec xy = g.Push(null, rect).rect.xy;
                        BigNumbers.Render(valueOrDefault4, xy.x, xy.y, textColor ?? Colors.textMain);
                        g.Pop();
                    }
                    w += text.Length * numberWidth;
                }
            }
            if (x.HasValue)
            {
                if (x < 0)
                {
                    w += iconNumberPadding;
                    if (!dontDraw)
                    {
                        Rect? rect = new Rect(w - 2);
                        Vec xy = g.Push(null, rect).rect.xy;
                        Color? color14 = (action.disabled ? new Color?(spriteColor) : textColor);
                        Draw.Sprite(Enum.Parse<Spr>("icons_minus"), xy.x, xy.y - 1.0, flipX: false, flipY: false, 0.0, null, null, null, null, color14);
                        g.Pop();
                    }
                    w += 3;
                }
                if (Math.Abs(x.Value) > 1)
                {
                    w += iconNumberPadding + 1;
                    if (!dontDraw)
                    {
                        G g18 = g;
                        Rect? rect12 = new Rect(w);
                        Vec xy16 = g18.Push(null, rect12).rect.xy;
                        BigNumbers.Render(Math.Abs(x.Value), xy16.x, xy16.y, textColor ?? Colors.textMain);
                        g.Pop();
                    }
                    w += 4;
                }
                w += iconNumberPadding;
                if (!dontDraw)
                {
                    G g19 = g;
                    Rect? rect12 = new Rect(w);
                    Vec xy17 = g19.Push(null, rect12).rect.xy;
                    Spr? id13 = Enum.Parse<Spr>("icons_x_white");
                    double x17 = xy17.x;
                    double y16 = xy17.y - 1.0;
                    Color? color14 = action.GetIcon(state)?.color;
                    Draw.Sprite(id13, x17, y16, flipX: false, flipY: false, 0.0, null, null, null, null, color14);
                    g.Pop();
                }
                w += 8;
            }
            isFirst = false;
        }
    }
}
