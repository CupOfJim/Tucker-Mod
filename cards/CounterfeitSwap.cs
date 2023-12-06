﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class CounterfeitSwap : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            Upgrade upgrade = base.upgrade;
            switch (upgrade)
            {
                case Upgrade.None:
                    return new List<CardAction> ()
                    {
                        new AShieldSteal() { amount = 3 },
                        new AStatus()
                        {
                            status = Enum.Parse<Status>("tempshield"),
                            statusAmount = 3,
                            targetPlayer = true
                        }
                    };
                case Upgrade.A:
                    return new List<CardAction> ()
                    {
                        new AShieldSteal() { amount = 4 },
                        new AStatus()
                        {
                            status = Enum.Parse<Status>("tempshield"),
                            statusAmount = 4,
                            targetPlayer = true
                        }
                    };
                case Upgrade.B:
                    return new List<CardAction> ()
                    {
                        new AStatus()
                        {
                            status = Enum.Parse<Status>("tempshield"),
                            statusAmount = 3,
                            targetPlayer = true
                        },
                        new AShieldSteal() { amount = 3 }
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