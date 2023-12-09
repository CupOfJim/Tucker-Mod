using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Juggle : Card
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
                            from = -1,
                            damage = GetDmg(s, 0),
                            fast = true,
                            moveEnemy = 2,
                        },
                        new AAttack ()
                        {
                            from = 1,
                            damage = GetDmg(s, 0),
                            fast = true,
                            moveEnemy = -2,
                        },
                        new AAttack ()
                        {
                            from = -1,
                            damage = GetDmg(s, 3),
                            fast = true,
                        }
                    };
                case Upgrade.A:
                    return new List<CardAction> ()
                    {
                        new AAttack ()
                        {
                            from = -1,
                            damage = GetDmg(s, 0),
                            fast = true,
                            moveEnemy = 2,
                        },
                        new AAttack ()
                        {
                            from = 1,
                            damage = GetDmg(s, 0),
                            fast = true,
                            moveEnemy = -2,
                        },
                        new AReplay(),
                        new AAttack ()
                        {
                            from = -1,
                            damage = GetDmg(s, 4),
                            fast = true,
                        }
                    };
                case Upgrade.B:
                    return new List<CardAction> ()
                    {
                        new AAttack ()
                        {
                            from = -2,
                            damage = GetDmg(s, 0),
                            fast = true,
                            moveEnemy = 2,
                        },
                        new AAttack ()
                        {

                            damage = GetDmg(s, 0),
                            fast = true,
                            moveEnemy = -2,
                        },
                        new AAttack ()
                        {
                            from = 2,
                            damage = GetDmg(s, 3),
                            fast = true,
                        }
                    };
            }
        }
        public override CardData GetData(State state)
        {
            return new()
            {
                cost = 1
            };
        }
    }
}
