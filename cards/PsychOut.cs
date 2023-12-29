using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using TuckerTheSaboteur.actions;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class PsychOut : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {

            List<CardAction> actions = new()
            {
                new AStatus ()
                {
                    status = Enum.Parse<Status>("autododgeLeft"),
                    statusAmount = base.upgrade == Upgrade.B ? 2 : 1,
                    targetPlayer = false
                },
                new AAttack ()
                {
                    damage = GetDmg(s, 1),
                    fast = base.upgrade == Upgrade.B
                }
            };

            if (base.upgrade == Upgrade.B)
            {
                var damage = GetDmg(s, 1);
                int cannonX = s.ship.parts.FindIndex((Part p) => p.type == PType.cannon && p.active);

                Icon attackIcon = ABluntAttack.DoWeHaveCannonsThough(s)
                    ? new Icon(Enum.Parse<Spr>("icons_attack"), damage, Colors.redd)
                    : new Icon(Enum.Parse<Spr>("icons_attackFail"), damage, Colors.attackFail);

                actions.Add(
                    new TuckerTheSaboteur.actions.ATooltipDummy()
                    {
                        tooltips = new() { }, // eventually put the tooltip for offset attacks here
                        icons = new()
                        {
                            new Icon(Enum.Parse<Spr>("icons_moveLeftEnemy"), 2, Colors.redd),
                            attackIcon
                        }
                    }
                );
                actions.Add(
                    new actions.AAttackNoIcon()
                    {
                        fromX = cannonX-2,
                        damage = damage,
                        fast = true,
                    }
                );
            }

            return actions;
        }
        public override CardData GetData(State state)
        {
            return new()
            {
                cost = (upgrade == Upgrade.A ? 1 : 2),
                exhaust = true
            };
        }
    }
}
