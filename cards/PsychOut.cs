using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using TuckerTheSaboteur.actions;
using TuckerMod.actions;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class PsychOut : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            List<CardAction> actions = new()
            {
                new AStatus ()
                {
                    status = Enum.Parse<Status>("autododgeLeft"),
                    statusAmount = base.upgrade == Upgrade.B ? 2 : 1,
                    targetPlayer = false
                },
                new AAttack ()
                {
                    damage = GetDmg(s, 1),
                    fast = base.upgrade == Upgrade.B
                }
            };

            if (base.upgrade == Upgrade.B)
            {
                int cannonX = s.ship.parts.FindIndex((Part p) => p.type == PType.cannon && p.active);

                actions.Add(
                    new AAttack()
                    {
                        fromX = cannonX - 2,
                        damage = GetDmg(s, 1),
                        fast = true,
                    }
                );
            }

            List<CardAction> finalActions = new();
            for (int i = 0; i < actions.Count; i++) finalActions.Add(new ADummyAction());
            finalActions.AddRange(actions.Select(a => ATooltipDummy.BuildStandIn(a, s)));
            finalActions.AddRange(actions.Select(a => new ANoIconWrapper() { action = a }));

            return finalActions;
        }
        public override CardData GetData(State state)
        {
            return new()
            {
                cost = (upgrade == Upgrade.A ? 1 : 2),
                exhaust = true,
                artTint = "ffffaa"
            };
        }
    }
}
