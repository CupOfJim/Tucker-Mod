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
                            fromX = -1,
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
                            fromX = 1,
                            damage = GetDmg(s, 1),
                            fast = true,
                        }
                    };
                case Upgrade.A:
                    return new List<CardAction> ()
                    {
                        new AAttack ()
                        {
                            fromX = -2,
                            damage = GetDmg(s, 1),
                            fast = true,
                        },
                        new AAttack () {
                            fromX = -1,
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
                            fromX = 1,
                            damage = GetDmg(s, 1),
                            fast = true,
                        },
                        new AAttack () {
                            fromX = 2,
                            damage = GetDmg(s, 1),
                            fast = true,
                        }
                    };
                case Upgrade.B:
                    return new List<CardAction> ()
                    {
                        new AAttack ()
                        {
                            fromX = -1,
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
                            fromX = 1,
                            damage = GetDmg(s, 1),
                            fast = true,
                            piercing = true,
                        }
                    };
            }

            throw new Exception(this.GetType().Name + " was upgraded to something that doesn't exist.");
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
