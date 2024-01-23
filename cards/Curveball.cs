using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur.actions;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Curveball : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            Upgrade upgrade = base.upgrade;

            // handle attack offset
            int cannonX = s.ship.parts.FindIndex((Part p) => p.type == PType.cannon && p.active);

            int offset = upgrade == Upgrade.B ? -4 : -2;
            int offsetAmount = Math.Abs(offset);

            Spr offsetSprite = (Spr)MainManifest.sprites["icons/Offset_Shot_Left"].Id;

            // handle attack damage
            int damage = GetDmg(s, 2);

            Icon attackIcon = ABluntAttack.DoWeHaveCannonsThough(s)
                ? new Icon(Enum.Parse<Spr>("icons_attack"), damage, Colors.redd)
                : new Icon(Enum.Parse<Spr>("icons_attackFail"), damage, Colors.attackFail);

            // return final result
            return new List<CardAction>()
            {
                new TuckerTheSaboteur.actions.AAttackNoIcon ()
                {
                    fromX = cannonX + offset,
                    damage = damage,
                },
                new TuckerTheSaboteur.actions.ATooltipDummy()
                {
                    tooltips = new() { }, // eventually put the tooltip for offset attacks here
                    icons = new()
                    {
                        new Icon(offsetSprite, offsetAmount, Colors.redd),
                        attackIcon,
                    }
                },
                new ADummyAction()
            };
        }
        public override CardData GetData(State state)
        {
            return new()
            {
                cost = (upgrade == Upgrade.B ? 0 : 1),
                flippable = (upgrade == Upgrade.A ? true : false),
                artTint = "ffffaa"
            };
        }
    }
}
