using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Cripple : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            Upgrade upgrade = base.upgrade;
            switch (upgrade)
            {
                case Upgrade.None:
                    return new List<CardAction> ()
                    {
                        new ABluntAttack ()
                        {
                            damage = GetDmg(s, 2),
                            status = Enum.Parse<Status>("fuelleak"),
                            statusAmount = 1,
                        }
                    };
                case Upgrade.A:
                    return new List<CardAction> ()
                    {
                        new AAttack ()
                        {
                            damage = GetDmg(s, 2),
                            status = Enum.Parse<Status>("fuelleak"),
                            statusAmount = 1,
                        }
                    };
                case Upgrade.B:
                    return new List<CardAction> ()
                    {
                        new ABluntAttack ()
                        {
                            damage = GetDmg(s, 1),
                            status = Enum.Parse<Status>("fuelleak"),
                            statusAmount = 1,
                        },
                        new AAttack ()
                        {
                            damage = GetDmg(s, 1),
                            status = Enum.Parse<Status>("fuelleak"),
                            statusAmount = 1,
                        }
                    };
            }
        }
        public override CardData GetData(State state)
        {
            return new()
            {
                cost = (upgrade == Upgrade.B ? 4 : 3),
                exhaust = true
            };
        }
    }
}
