using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class PsychOut : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            Upgrade upgrade = base.upgrade;
            switch (upgrade)
            {
                case Upgrade.None:
                    return new List<CardAction> ()
                    {
                        new AStatus ()
                        {
                            status = Enum.Parse<Status>("autododgeLeft"),
                            statusAmount = 1,
                            targetPlayer = false
                        }
                        new AAttack ()
                        {
                            damage = GetDmg(s, 1),
                            fast = true,
                        }
                    };
                case Upgrade.A:
                    return new List<CardAction> ()
                    {
                        new AStatus ()
                        {
                            status = Enum.Parse<Status>("autododgeLeft"),
                            statusAmount = 1,
                            targetPlayer = false
                        }
                        new AAttack ()
                        {
                            damage = GetDmg(s, 1),
                            fast = true,
                        }
                    };
                case Upgrade.B:
                    return new List<CardAction> ()
                    {
                        new AStatus ()
                        {
                            status = Enum.Parse<Status>("autododgeLeft"),
                            statusAmount = 2,
                            targetPlayer = false
                        }
                        new AAttack ()
                        {
                            damage = GetDmg(s, 1),
                            fast = true,
                        }
                        new AAttack ()
                        {
                            from = -2
                            damage = GetDmg(s, 1),
                            fast = true,
                        }
                    };
            };
        }
        public override CardData GetData(State state)
        {
            return new()
            {
                cost = (upgrade == Upgrade.A ? 1 : 2),
                exhaust = true
            };
        }
    }
}
