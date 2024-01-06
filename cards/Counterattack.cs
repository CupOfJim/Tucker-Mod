using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.common, dontOffer = true, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Counterattack : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            Upgrade upgrade = base.upgrade;
            switch (upgrade)
            {
                case Upgrade.None:
                    return new List<CardAction> ()
                    {
                        new AAttack ()
                        {
                            damage = GetDmg(s, 2),
                        }
                    };
                case Upgrade.A:
                    return new List<CardAction> ()
                    {
                        new AAttack ()
                        {
                            damage = GetDmg(s, 3),
                        }
                    };
                case Upgrade.B:
                    return new List<CardAction> ()
                    {
                        new AAttack ()
                        {
                            damage = GetDmg(s, 2),
                        }
                    };
            }

            throw new Exception(this.GetType().Name + " was upgraded to something that doesn't exist.");
        }
        public override CardData GetData(State state)
        {
            return new()
            {
                cost = 0,
                temporary = true,
                exhaust = (upgrade == Upgrade.B ? false : true),
                artTint = "ffffaa"
            };
        }
    }
}
