using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerMod.actions;
using TuckerTheSaboteur.actions;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Hook : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            int cannonX = s.ship.parts.FindIndex((Part p) => p.type == PType.cannon && p.active);
            int offset = base.upgrade == Upgrade.B ? -1 : -3;

            List<CardAction> actions = new()
            {
                new AAttack()
                {
                    fromX = cannonX + offset,
                    damage = GetDmg(s, 2),
                    moveEnemy = 2,
                }
            };

            return ATooltipDummy.BuildStandinsAndWrapRealActions(actions, s);
        }
        public override CardData GetData(State state)
        {
            return new()
            {
                cost = 1,
                flippable = upgrade == Upgrade.A ? true : false,
                artTint = "ffffaa"
            };
        }
    }
}
