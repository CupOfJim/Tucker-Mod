using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerMod.actions;
using TuckerTheSaboteur.actions;
using static System.Net.Mime.MediaTypeNames;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.rare, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class ThreadtheNeedle : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            int cannonX = s.ship.parts.FindIndex((Part p) => p.type == PType.cannon && p.active);

            int offset = base.upgrade == Upgrade.B ? -1 : 2;
            int statusAmount = base.upgrade == Upgrade.B ? 3: 2;

            List<CardAction> actions = new()
            {
                new AAttack ()
                {
                    fromX = cannonX+offset,
                    damage = GetDmg(s, 0),
                    status = Enum.Parse<Status>("tempShield"),
                    statusAmount = statusAmount,
                    fast = true,
                },
                new AAttack ()
                {
                    damage = GetDmg(s, base.upgrade == Upgrade.A ? 4 : 3),
                    fast = true,
                }
            };

            if (base.upgrade != Upgrade.B)
            {
                actions.Insert(0,
                    new AAttack()
                    {
                        fromX = cannonX - offset,
                        damage = GetDmg(s, 0),
                        status = Enum.Parse<Status>("tempShield"),
                        statusAmount = statusAmount,
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
                cost = 1,
                flippable = (upgrade == Upgrade.B ? true : false),
                artTint = "ffffaa"
            };
        }
    }
}
