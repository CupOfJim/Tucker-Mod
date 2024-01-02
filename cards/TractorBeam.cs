using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur.actions;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class TractorBeam : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            return new()
            {
               new AShieldSteal() { amount = 2 }
            };
        }
        public override CardData GetData(State state)
        {
            return new()
            {
                cost = 0,
                exhaust = (upgrade == Upgrade.A ? false : true),
                buoyant = (upgrade == Upgrade.B ? true : false),
                artTint = "ffffaa"
            };
        }
    }
}
