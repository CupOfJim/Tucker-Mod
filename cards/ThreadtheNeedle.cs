using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur.actions;
using static System.Net.Mime.MediaTypeNames;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class ThreadtheNeedle : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {

            Spr attackSpr = ABluntAttack.DoWeHaveCannonsThough(s)
                ? Enum.Parse<Spr>("icons_attack")
                : Enum.Parse<Spr>("icons_attackFail");

            int cannonX = s.ship.parts.FindIndex((Part p) => p.type == PType.cannon && p.active);

            int offset = base.upgrade == Upgrade.B ? -1 : 2;
            int statusAmount = base.upgrade == Upgrade.None ? 3: 4;
            int damage3 = base.upgrade == Upgrade.A ? 4 : 6;

            List<CardAction> actions = new()
            {
                new AAttackNoIcon ()
                {
                    fromX = cannonX+offset,
                    damage = GetDmg(s, 0),
                    status = Enum.Parse<Status>("tempShield"),
                    statusAmount = statusAmount,
                    fast = true,
                },
                new AAttackNoIcon ()
                {
                    damage = GetDmg(s, damage3),
                    fast = true,
                }
            };

            if (base.upgrade != Upgrade.B)
            {
                actions.Insert(0,
                    new AAttackNoIcon()
                    {
                        fromX = cannonX - offset,
                        damage = GetDmg(s, 0),
                        status = Enum.Parse<Status>("tempShield"),
                        statusAmount = statusAmount,
                        fast = true,
                    }
                );

                actions.Add(
                    new TuckerTheSaboteur.actions.ATooltipDummy()
                    {
                        tooltips = new() { }, // eventually put the tooltip for offset attacks here
                        icons = new()
                        {
                            new Icon((Spr)MainManifest.sprites["icons/Offset_Shot_Left"].Id, offset, Colors.redd),
                            new Icon(Enum.Parse<Spr>("icons_attack"), GetDmg(s, 0), Colors.redd),
                            new Icon(Enum.Parse<Spr>("icons_outgoing"), null, Colors.textMain),
                            new Icon(Enum.Parse<Spr>("icons_tempShield"), statusAmount, Colors.textMain)
                        }
                    }
                );
            }

            actions.Add(
                new TuckerTheSaboteur.actions.ATooltipDummy()
                {
                    tooltips = new() { }, // eventually put the tooltip for offset attacks here
                    icons = new()
                    {
                            new Icon((Spr)MainManifest.sprites["icons/Offset_Shot_Left"].Id, offset, Colors.redd),
                            new Icon(Enum.Parse<Spr>("icons_attack"), GetDmg(s, 0), Colors.redd),
                            new Icon(Enum.Parse<Spr>("icons_outgoing"), null, Colors.textMain),
                            new Icon(Enum.Parse<Spr>("icons_tempShield"), statusAmount, Colors.textMain)
                    }
                }
            );

            actions.Add(
                new TuckerTheSaboteur.actions.ATooltipDummy()
                {
                    tooltips = new() { }, // eventually put the tooltip for offset attacks here
                    icons = new()
                    {
                        new Icon(Enum.Parse<Spr>("icons_attack"), GetDmg(s, damage3), Colors.redd),
                    }
                }
            );

            return actions;
        }
        public override CardData GetData(State state)
        {
            return new()
            {
                cost = 1,
                flippable = (upgrade == Upgrade.B ? true : false)
            };
        }
    }
}
