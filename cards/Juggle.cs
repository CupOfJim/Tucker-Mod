using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerMod.actions;
using TuckerTheSaboteur.actions;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Juggle : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            int cannonX = s.ship.parts.FindIndex((Part p) => p.type == PType.cannon && p.active);

            List<CardAction> actions = new();
            actions.Add(
                new AAttack()
                {
                    fromX = cannonX + (base.upgrade == Upgrade.B ? 2 : -1),
                    damage = GetDmg(s, 0),
                    fast = true,
                    moveEnemy = (base.upgrade == Upgrade.B ? -2 : 2),
                }
            );
            actions.Add(
                new AAttack()
                {
                    fromX = cannonX + (base.upgrade == Upgrade.B ? 0 : 1),
                    damage = GetDmg(s, 0),
                    fast = true,
                    moveEnemy = -2,
                }
            );
            actions.Add(
                new AAttack()
                {
                    fromX = cannonX + (base.upgrade == Upgrade.B ? -2 : -1),
                    damage = GetDmg(s, (base.upgrade == Upgrade.A ? 4 : 3)),
                    fast = true,
                }
            );

            return ATooltipDummy.BuildStandinsAndWrapRealActions(actions, s);
        }
        public override CardData GetData(State state)
        {
            return new()
            {
                cost = 2,
                artTint = "ffffaa"
            };
        }
    }
}
