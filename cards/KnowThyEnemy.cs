﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class KnowThyEnemy : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            Upgrade upgrade = base.upgrade;
            switch (upgrade)
            {
                case Upgrade.None:
                    return new List<CardAction> ()
                    {
                        new AAttack
                        {
                            damage = GetDmg(s, 1),
                            piercing = true,
                            disabled = flipped,
                        },
                        new AStatus()
                        {
                            status = Enum.Parse<Status>("shield"),
                            statusAmount = -1,
                            targetPlayer = false,
                            disabled = flipped,
                        },
                        new ADummyAction(),
                        new ABluntAttack ()
                        {
                            damage = GetDmg(s, 3),
                            disabled = !flipped,
                        }
                    };
                case Upgrade.A:
                    return new List<CardAction> ()
                    {
                        new AAttack
                        {
                            damage = GetDmg(s, 1),
                            piercing = true,
                            disabled = flipped,
                        },
                        new AShieldSteal()
                        {
                            amount = 1,
                            disabled = flipped,
                        },
                        new ADummyAction(),
                        new ABluntAttack ()
                        {
                            damage = GetDmg(s, 3),
                            disabled = !flipped,
                        }
                    };
                case Upgrade.B:
                    return new List<CardAction> ()
                    {
                        new AAttack
                        {
                            damage = GetDmg(s, 1),
                            piercing = true,
                            disabled = flipped,
                        },
                        new AStatus()
                        {
                            status = Enum.Parse<Status>("shield"),
                            statusAmount = -2,
                            targetPlayer = false,
                            disabled = flipped,
                        },
                        new ADummyAction(),
                        new ABluntAttack ()
                        {
                            damage = GetDmg(s, 4),
                            disabled = !flipped,
                        }
                    };
            }
        }
        public override CardData GetData(State state)
        {
            return new()
            {
                cost = 1,
                floppable = true
            };
        }
    }
}