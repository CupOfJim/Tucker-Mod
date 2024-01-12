using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerMod.actions;
using TuckerTheSaboteur.actions;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Hook : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            int cannonX = s.ship.parts.FindIndex((Part p) => p.type == PType.cannon && p.active);

            int offset = base.upgrade == Upgrade.B ? -1 : -3;
            if (flipped) { offset *= -1; }

            int moveDistance = 2;
            if (flipped) { moveDistance *= -1; }

            List<CardAction> actions = new()
            {
                new TuckerTheSaboteur.actions.AAttackNoIcon ()
                {
                    fromX = cannonX + offset,
                    damage = GetDmg(s, 2),
                    moveEnemy = moveDistance,
                }
            };

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
                flippable = (upgrade == Upgrade.A ? true : false) || (state.ship.Get(Enum.Parse<Status>("tableFlip")) > 0),
                artTint = "ffffaa"
            };
        }
    }
}
