﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class LieLow : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            Upgrade upgrade = base.upgrade;
            switch (upgrade)
            {
                case Upgrade.None:
                    return new List<CardAction> ()
                    {
                        new AStatus() {
                            status = Enum.Parse<Status>("maxShield"),
                            targetPlayer = true,
                            statusAmount = 1,
                            mode = AStatusMode.Add,
                        },
                        new AStatus ()
                        {
                            status = Enum.Parse<Status>("shield"),
                            statusAmount = 2,
                            targetPlayer = true
                        },
                        new AStatus ()
                        {
                            status = Enum.Parse<Status>("lockdown"),
                            statusAmount = 2,
                            targetPlayer = true
                        }
                    };
                case Upgrade.A:
                    return new List<CardAction> ()
                    {
                        new AStatus() {
                            status = Enum.Parse<Status>("maxShield"),
                            targetPlayer = true,
                            statusAmount = 1,
                            mode = AStatusMode.Add,
                        },
                        new AStatus ()
                        {
                            status = Enum.Parse<Status>("shield"),
                            statusAmount = 3,
                            targetPlayer = true
                        },
                        new AStatus ()
                        {
                            status = Enum.Parse<Status>("lockdown"),
                            statusAmount = 2,
                            targetPlayer = true
                        }
                    };
                case Upgrade.B:
                    return new List<CardAction> ()
                    {
                        new AStatus() {
                            status = Enum.Parse<Status>("maxShield"),
                            targetPlayer = true,
                            statusAmount = 1,
                            mode = AStatusMode.Add,
                        },
                        new AStatus ()
                        {
                            status = Enum.Parse<Status>("shield"),
                            statusAmount = 2,
                            targetPlayer = true
                        },
                        new AStatus ()
                        {
                            status = Enum.Parse<Status>("lockdown"),
                            statusAmount = 3,
                            targetPlayer = true
                        }
                    };
            }

            throw new Exception(this.GetType().Name + " was upgraded to something that doesn't exist.");
        }
        public override CardData GetData(State state)
        {
            return new()
            {
                cost = (upgrade == Upgrade.B ? 0 : 1),
                artTint = "ffffaa"
            };
        }
    }
}
