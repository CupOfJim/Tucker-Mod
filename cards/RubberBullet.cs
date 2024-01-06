using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur.actions;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class RubberBullet : Card
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
                            damage = GetDmg(s, 2),
                            stunEnemy = true
                        }
                    };
                case Upgrade.A:
                    return new List<CardAction> ()
                    {
                        new ABluntAttack ()
                        {
                            damage = GetDmg(s, 3),
                            stunEnemy = true
                        }
                    };
                case Upgrade.B:
                    return new List<CardAction> ()
                    {
                        new AStatus()
                        {
                            status = Enum.Parse<Status>("shield"),
                            statusAmount = -2,
                            targetPlayer = false,
                        },
                        new ABluntAttack ()
                        {
                            damage = GetDmg(s, 2),
                            stunEnemy = true
                        }
                    };
            }

            throw new Exception(this.GetType().Name + " was upgraded to something that doesn't exist.");
        }
        public override CardData GetData(State state)
        {
            return new()
            {
                cost = 1,
                artTint = "ffffaa"
            };
        }
    }
}
