using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhilipTheMechanic.cards
{
    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class MutualGain : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            if (this.upgrade == Upgrade.B)
            {

                return new()
                {
                    new AStatus() {
                        //status = (Status)MainManifest.statuses["redraw"].Id, // Example of applying a custom status
                        status = Enum.Parse<Status>("maxShield"),
                        targetPlayer = false,
                        statusAmount = 1,
                        mode = AStatusMode.Add,
                    },
                    new AStatus() {
                        status = Enum.Parse<Status>("shield"),
                        targetPlayer = false,
                        statusAmount = 3,
                        mode = AStatusMode.Add,
                    },
                    new AStatus() {
                        status = Enum.Parse<Status>("maxShield"),
                        targetPlayer = true,
                        statusAmount = 1,
                        mode = AStatusMode.Add,
                    },
                    new AStatus() {
                        status = Enum.Parse<Status>("shield"),
                        targetPlayer = true,
                        statusAmount = 3,
                        mode = AStatusMode.Add,
                    },

                };
            }
            else
            {
                return new()
                {
                    new AStatus() {
                        status = Enum.Parse<Status>("shield"),
                        targetPlayer = false,
                        statusAmount = 3,
                        mode = AStatusMode.Add,
                    },
                    new AStatus() {
                        status = Enum.Parse<Status>("shield"),
                        targetPlayer = true,
                        statusAmount = 3,
                        mode = AStatusMode.Add,
                    },

                };
            }
        }

        public override CardData GetData(State state)
        {
            return new()
            {
                cost = (upgrade == Upgrade.A ? 0 : 1)
            };
        }
    }
}
