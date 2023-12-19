using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur.actions;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Sabotage : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            Upgrade upgrade = base.upgrade;
            switch (upgrade)
            {
                case Upgrade.None:
                    return new List<CardAction> ()
                    {
                        new AShieldSteal() { amount = 2 },
                    };
                case Upgrade.A:
                    return new List<CardAction> ()
                    {
                        new AShieldSteal() { amount = 2 },
                        new AStatus()
                        {
                            status = Enum.Parse<Status>("shield"),
                            statusAmount = -1,
                            targetPlayer = false
                        }
                    };
                case Upgrade.B:
                    return new List<CardAction> ()
                    {
                        new AShieldSteal() { amount = 2 },
                        new AStatus()
                        {
                            status = Enum.Parse<Status>("shield"),
                            statusAmount = 1,
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
                cost = 1
            };
        }
    }
}
