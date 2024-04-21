using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur.actions;
using static System.Net.Mime.MediaTypeNames;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class SprayandPray : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            Upgrade upgrade = base.upgrade;

            // handle attack offset
            int cannonX = s.ship.parts.FindIndex((Part p) => p.type == PType.cannon && p.active);

            // return final result
            List<CardAction> actions = new();

            if (upgrade == Upgrade.A)
            {
                actions.Add(new AAttack
                {
                    fromX = cannonX - 2,
                    damage = GetDmg(s, 1),
                    fast = true,
                });
            }

            actions.Add(new AAttack
            {
                fromX = cannonX - 1,
                damage = GetDmg(s, 1),
                fast = true,
                piercing = (upgrade == Upgrade.B),
            });
            actions.Add(new AAttack
            {
                damage = GetDmg(s, 2),
                fast = true,
                piercing = (upgrade == Upgrade.B),
            });
            actions.Add(new AAttack
            {
                fromX = cannonX + 1,
                damage = GetDmg(s, 1),
                fast = true,
                piercing = (upgrade == Upgrade.B),
            });

            if (upgrade == Upgrade.A)
            {
                actions.Add(new AAttack
                {
                    fromX = cannonX + 2,
                    damage = GetDmg(s, 1),
                    fast = true,
                });
            }

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
