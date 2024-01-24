using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur.actions;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Cripple : Card
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
                            status = (Status)MainManifest.statuses["fuel_leak"].Id,
                            statusAmount = 1,
                        }
                    };
                case Upgrade.A:
                    return new List<CardAction> ()
                    {
                        new ABluntAttack ()
                        {
                            damage = GetDmg(s, 2),
                            status = (Status)MainManifest.statuses["fuel_leak"].Id,
                            statusAmount = 1,
                        }
                    };
                case Upgrade.B:
                    return new List<CardAction> ()
                    {
                        new ABluntAttack ()
                        {
                            damage = GetDmg(s, 1),
                            status = (Status)MainManifest.statuses["fuel_leak"].Id,
                            statusAmount = 1,
                        },
                        new AAttack ()
                        {
                            damage = GetDmg(s, 1),
                            status = (Status)MainManifest.statuses["fuel_leak"].Id,
                            statusAmount = 1,
                        }
                    };
            }

            throw new Exception(this.GetType().Name + " was upgraded to something that doesn't exist.");
        }
        public override CardData GetData(State state)
        {
            return new()
            {
                cost = upgrade switch
                {
                    Upgrade.None => 3,
                    Upgrade.A => 2,
                    Upgrade.B => 4,
                },
                artTint = "ffffaa"
            };
        }
    }
}
