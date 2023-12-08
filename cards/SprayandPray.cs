using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class SprayandPray : Card
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
                            damage = GetDmg(s, 1),
                            fast = true,
                        },
                        new AAttack ()
                        {
                            damage = GetDmg(s, 2),
                            fast = true,
                        },
                        new AAttack ()
                        {
                            from = 1,
                            damage = GetDmg(s, 1),
                            fast = true,
                        }
                    };
                case Upgrade.A:
                    return new List<CardAction> ()
                    {
                        new AAttack ()
                        {
                            from = -2,
                            damage = GetDmg(s, 1),
                            fast = true,
                        },
                        {
                            from = -1,
                            damage = GetDmg(s, 1),
                            fast = true,
                        },
                        new AAttack ()
                        {
                            damage = GetDmg(s, 2),
                            fast = true,
                        },
                        new AAttack ()
                        {
                            from = 1,
                            damage = GetDmg(s, 1),
                            fast = true,
                        },
                        {
                            from = 2,
                            damage = GetDmg(s, 1),
                            fast = true,
                        }
                    };
                case Upgrade.B:
                    return new List<CardAction> ()
                    {
                        new AAttack ()
                        {
                            from = -1,
                            damage = GetDmg(s, 1),
                            fast = true,
                            piercing = true,
                        },
                        new AAttack ()
                        {
                            damage = GetDmg(s, 2),
                            fast = true,
                            piercing = true,
                        },
                        new AAttack ()
                        {
                            from = 1,
                            damage = GetDmg(s, 1),
                            fast = true,
                            piercing = true,
                        }
                    };
            }
        }
        public override CardData GetData(State state)
        {
            return new()
            {
                cost = 2
            };
        }
    }
}
