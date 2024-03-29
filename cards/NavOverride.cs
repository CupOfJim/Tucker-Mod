﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class NavOverride : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            Upgrade upgrade = base.upgrade;
            switch (upgrade)
            {
                case Upgrade.None:
                    return new List<CardAction> ()
                    {
                        new AMove ()
                        {
                            dir = -1,
                            targetPlayer = false
                        },
                        new AAddCard
                        {
                            card = new TuckerMisdirection(),
                            destination = Enum.Parse<CardDestination>("Hand")
                        }
                    };
                case Upgrade.A:
                    return new List<CardAction> ()
                    {
                        new AMove ()
                        {
                            dir = -2,
                            targetPlayer = false
                        },
                        new AAddCard
                        {
                            card = new TuckerMisdirection(),
                            destination = Enum.Parse<CardDestination>("Hand")
                        }
                    };
                case Upgrade.B:
                    return new List<CardAction> ()
                    {
                        new AAddCard
                        {
                            card = new TuckerMisdirection(),
                            destination = Enum.Parse<CardDestination>("Hand")
                        },
                        new AAddCard
                        {
                            card = new TuckerMisdirection(),
                            destination = Enum.Parse<CardDestination>("Hand")
                        }
                    };
            }

            throw new Exception(this.GetType().Name + " was upgraded to something that doesn't exist.");
        }
        public override CardData GetData(State state)
        {
            string desc = "";
            switch (this.upgrade)
            {
                case Upgrade.None:
                    desc = $"Move the enemy 1 space to the {(flipped ? "right": "left")}. Gain a <c=card>Redirect</c>."; break;
                case Upgrade.A:
                    desc = $"Move the enemy 2 spaces to the {(flipped ? "right": "left")}. Gain a <c=card>Redirect</c>."; break;
                case Upgrade.B:
                    desc = $"Gain 2 <c=card>Redirects</c>."; break;
            }
            return new()
            {
                cost = 1,
                flippable = (this.upgrade != Upgrade.B),
                description = desc,
                artTint = "ffffaa"
            };
        }
    }
}
