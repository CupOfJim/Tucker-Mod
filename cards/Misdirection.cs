﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.uncommon, dontOffer = true, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Misdirection : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            Upgrade upgrade = base.upgrade;
            switch (upgrade)
            {
                case Upgrade.None:
                    return new List<CardAction>()
                    {
                        new AMove ()
                        {
                            dir = -1,
                            targetPlayer = false
                        }
                    };
                case Upgrade.A:
                    return new List<CardAction>()
                    {
                        new AMove ()
                        {
                            dir = -2,
                            targetPlayer = false
                        }
                    };
                case Upgrade.B:
                    return new List<CardAction>()
                    {
                        new AMove ()
                        {
                            dir = -1,
                            targetPlayer = false
                        },
                        new ADrawCard
                        {
                            count = 1
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
                exhaust = true,
                retain = true,
                flippable = true
            };
        }
    }
}