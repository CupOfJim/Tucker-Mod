﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class FortressMode : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            Upgrade upgrade = base.upgrade;
            switch (upgrade)
            {
                case Upgrade.None:
                    return new List<CardAction> ()
                    {
                        new AStatus ()
                        {
                            status = (Status)MainManifest.statuses["buffer"].Id,
                            statusAmount = 3,
                            targetPlayer = true
                        },
                        new AStatus ()
                        {
                            status = Enum.Parse<Status>("lockdown"),
                            statusAmount = 3,
                            targetPlayer = true
                        }
                    };
                case Upgrade.A:
                    return new List<CardAction> ()
                    {
                        new AStatus ()
                        {
                            status = (Status)MainManifest.statuses["buffer"].Id,
                            statusAmount = 4,
                            targetPlayer = true
                        },
                        new AStatus ()
                        {
                            status = Enum.Parse<Status>("lockdown"),
                            statusAmount = 3,
                            targetPlayer = true
                        }
                    };
                case Upgrade.B:
                    return new List<CardAction> ()
                    {
                        new AStatus ()
                        {
                            status = (Status)MainManifest.statuses["buffer"].Id,
                            statusAmount = 3,
                            targetPlayer = true
                        },
                        new AStatus ()
                        {
                            status = Enum.Parse<Status>("engineStall"),
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
                cost = 2,
                artTint = "ffffaa"
            };
        }
    }
}
