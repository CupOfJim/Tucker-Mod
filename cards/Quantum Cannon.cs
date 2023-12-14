using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class MutualGain : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            Upgrade upgrade = base.upgrade;
            switch (upgrade)
            {
                case Upgrade.None:
                    return new List<CardAction>
            {
                new AAttack
                {
                    damage = GetDmg(s, 1),
                    fast = true,
                    piercing = true
                }
            };
                case Upgrade.A:
                    return new List<CardAction>
            {
                new AAttack
                {
                    damage = GetDmg(s, 2),
                    fast = true,
                    piercing = true
                }
            };
                case Upgrade.B:
                    return new List<CardAction>
            {
                new AAttack
                {
                    damage = GetDmg(s, 3),
                    status = Status.shield,
                    statusAmount = 2
                    fast = true,
                    piercing = true
                }
            }
}
