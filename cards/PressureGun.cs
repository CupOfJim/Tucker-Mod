using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class PressureGun : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            Upgrade upgrade = base.upgrade;
            switch (upgrade)
            {
                case Upgrade.None:
                    return new List<CardAction> ()
                    {
                        new ABluntAttack ()
                        {
                            damage = GetDmg(s, 0),
                            moveEnemy = -3,
                        }
                    };
                case Upgrade.A:
                    return new List<CardAction> ()
                    {
                        new ABluntAttack ()
                        {
                            damage = GetDmg(s, 0),
                            moveEnemy = -3,
                        }
                    };
                case Upgrade.B:
                    return new List<CardAction> ()
                    {
                        new AAttack ()
                        {
                            damage = GetDmg(s, 0),
                            moveEnemy = -3,
                        }
                    };
            }
        }
        public override CardData GetData(State state)
        {
            return new()
            {
                cost = 0,
                flippable = (upgrade == Upgrade.A ? true : false)
            };
        }
    }
}
