using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class ThreadtheNeedle : Card
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
                            fromX = -2,
                            damage = GetDmg(s, 0),
                            status = Enum.Parse<Status>("tempshield"),
                            statusAmount = 3,
                            fast = true,
                        },
                        new AAttack ()
                        {
                            fromX = 2,
                            damage = GetDmg(s, 0),
                            status = Enum.Parse<Status>("tempshield"),
                            statusAmount = 3,
                            fast = true,
                        },
                        new AAttack ()
                        {
                            damage = GetDmg(s, 4),
                            fast = true,
                        }
                    };
                case Upgrade.A:
                    return new List<CardAction> ()
                    {
                        new AAttack ()
                        {
                            fromX = -2,
                            damage = GetDmg(s, 0),
                            status = Enum.Parse<Status>("tempshield"),
                            statusAmount = 4,
                            fast = true,
                        },
                        new AAttack ()
                        {
                            fromX = 2,
                            damage = GetDmg(s, 0),
                            status = Enum.Parse<Status>("tempshield"),
                            statusAmount = 4,
                            fast = true,
                        },
                        new AAttack ()
                        {
                            damage = GetDmg(s, 6),
                            fast = true,
                        }
                    };
                case Upgrade.B:
                    return new List<CardAction> ()
                    {
                        new AAttack ()
                        {
                            fromX = -1,
                            damage = GetDmg(s, 0),
                            status = Enum.Parse<Status>("tempshield"),
                            statusAmount = 4,
                            fast = true,
                        },
                        new AAttack ()
                        {
                            damage = GetDmg(s, 4),
                            fast = true,
                        }
                    };
            }

            throw new Exception(this.GetType().Name + " was upgraded to something that doesn't exist.");
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
