using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Curveball : Card
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
                            fromX = -2,
                            damage = GetDmg(s, 2),
                            fast = true,
                        }
                    };
                case Upgrade.A:
                    return new List<CardAction> ()
                    {
                        new AAttack ()
                        {
                            fromX = -2,
                            damage = GetDmg(s, 2),
                            fast = true,
                        }
                    };
                case Upgrade.B:
                    return new List<CardAction> ()
                    {
                        new AAttack ()
                        {
                            fromX = -4,
                            damage = GetDmg(s, 2),
                            fast = true,
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
                flippable = (upgrade == Upgrade.A ? true : false)
            };
        }
    }
}
