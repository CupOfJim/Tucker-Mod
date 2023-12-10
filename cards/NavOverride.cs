using System;
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
                            card = new Misdirection(),
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
                            card = new Misdirection(),
                            destination = Enum.Parse<CardDestination>("Hand")
                        }
                    };
                case Upgrade.B:
                    return new List<CardAction> ()
                    {
                        new AAddCard
                        {
                            card = new Misdirection(),
                            destination = Enum.Parse<CardDestination>("Hand")
                        },
                        new AAddCard
                        {
                            card = new Misdirection(),
                            destination = Enum.Parse<CardDestination>("Hand")
                        }
                    };
            }
        }
        public override CardData GetData(State state)
        {
            return new()
            {
                cost = 1,
                flippable = (upgrade == Upgrade.B ? false : true)
            };
        }
    }
}
