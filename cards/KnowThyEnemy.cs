using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur.actions;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class KnowThyEnemy : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            Upgrade upgrade = base.upgrade;
            switch (upgrade)
            {
                case Upgrade.None:
                    return new List<CardAction> ()
                    {
                        new AAttack
                        {
                            damage = GetDmg(s, 2),
                            piercing = true,
                            disabled = flipped,
                        },
                        new AStatus()
                        {
                            status = Enum.Parse<Status>("shield"),
                            statusAmount = -2,
                            targetPlayer = false,
                            disabled = flipped,
                        },
                        new ADummyAction(),
                        new ABluntAttack ()
                        {
                            damage = GetDmg(s, 5),
                            disabled = !flipped,
                        }
                    };
                case Upgrade.A:
                    return new List<CardAction> ()
                    {
                        new AAttack
                        {
                            damage = GetDmg(s, 3),
                            piercing = true,
                            disabled = flipped,
                        },
                        new AShieldSteal()
                        {
                            amount = 2,
                            disabled = flipped,
                        },
                        new ADummyAction(),
                        new ABluntAttack ()
                        {
                            damage = GetDmg(s, 5),
                            disabled = !flipped,
                        }
                    };
                case Upgrade.B:
                    return new List<CardAction> ()
                    {
                        new AAttack
                        {
                            damage = GetDmg(s, 2),
                            piercing = true,
                            disabled = flipped,
                        },
                        new AStatus()
                        {
                            status = Enum.Parse<Status>("shield"),
                            statusAmount = -3,
                            targetPlayer = false,
                            disabled = flipped,
                        },
                        new ADummyAction(),
                        new ABluntAttack ()
                        {
                            damage = GetDmg(s, 6),
                            disabled = !flipped,
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
                floppable = true,
                art = flipped ? (Spr)MainManifest.sprites["cards/MiningDrill_Bottom"].Id : (Spr)MainManifest.sprites["cards/MiningDrill_Top"].Id,
                artTint = "ffffff"
            };
        }
    }
}
