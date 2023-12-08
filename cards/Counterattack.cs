using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.common, dontOffer = true, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Counterattack : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            Upgrade upgrade = base.upgrade;
            switch (upgrade)
            {
                case Upgrade.None:
                    return new List<CardAction> ()
                    {
                        new AAttack ()
                        {
                            damage = GetDmg(s, 2),
                            fast = true,
                        }
                    };
                case Upgrade.A:
                    return new List<CardAction> ()
                    {
                        new AAttack ()
                        {
                            damage = GetDmg(s, 3),
                            fast = true,
                        }
                    };
                case Upgrade.B:
                    return new List<CardAction> ()
                    {
                        new AAttack ()
                        {
                            damage = GetDmg(s, 2),
                            fast = true,
                        }
                    };
            }
        }
        public override CardData GetData(State state)
        {
            return new()
            {
                cost = 0,
                temporary = true,
                exhaust = (upgrade == Upgrade.B ? false : true)
            };
        }
    }
}
