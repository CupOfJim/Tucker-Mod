using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur.actions;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.common, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Hook : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            Upgrade upgrade = base.upgrade;

            // handle attack offset
            int cannonX = s.ship.parts.FindIndex((Part p) => p.type == PType.cannon && p.active);

            int offset = upgrade == Upgrade.A ? -3 : -1;
            int offsetAmount = Math.Abs(offset);
            if (flipped) { offset *= -1; }

            Spr offsetSprite = flipped 
                ? (Spr)MainManifest.sprites["icons/Offset_Shot_Right"].Id
                : (Spr)MainManifest.sprites["icons/Offset_Shot_Left"].Id;

            // handle attack damage
            int damage = GetDmg(s, 2);

            Icon attackIcon = ABluntAttack.DoWeHaveCannonsThough(s)
                ? new Icon(Enum.Parse<Spr>("icons_attack"), damage, Colors.redd)
                : new Icon(Enum.Parse<Spr>("icons_attackFail"), damage, Colors.attackFail);

            // handle move enemy
            int moveDistance = 2;
            int moveAmount = Math.Abs(moveDistance);
            if (flipped) { moveDistance *= -1; }

            Spr moveSprite = flipped
                ? Enum.Parse<Spr>("icons_moveLeftEnemy")
                : Enum.Parse<Spr>("icons_moveRightEnemy");

            // return final result
            return new List<CardAction>()
            {
                new TuckerTheSaboteur.actions.AAttackNoIcon ()
                {
                    fromX = cannonX + offset,
                    damage = damage,
                    moveEnemy = moveDistance,
                },
                new TuckerTheSaboteur.actions.ATooltipDummy()
                {
                    tooltips = new() { }, // eventually put the tooltip for offset attacks here
                    icons = new()
                    {
                        new Icon(offsetSprite, offsetAmount, Colors.redd),
                        attackIcon,
                        new Icon(moveSprite, moveAmount, Colors.redd),
                    }
                },
                new ADummyAction()
            };
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
