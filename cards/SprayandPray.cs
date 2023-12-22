using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur.actions;
using static System.Net.Mime.MediaTypeNames;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class SprayandPray : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            Upgrade upgrade = base.upgrade;

            // handle attack offset
            int cannonX = s.ship.parts.FindIndex((Part p) => p.type == PType.cannon && p.active);

            // return final result
            List<CardAction> actions = new();

            if (upgrade == Upgrade.A)
            {
                actions.Add(new TuckerTheSaboteur.actions.AAttackNoIcon()
                {
                    fromX = cannonX - 2,
                    damage = GetDmg(s, 1),
                    fast = true,
                    piercing = true,
                });
            }

            actions.Add(new TuckerTheSaboteur.actions.AAttackNoIcon()
            {
                fromX = cannonX - 1,
                damage = GetDmg(s, 1),
                fast = true,
                piercing = (upgrade == Upgrade.B),
            });
            actions.Add(new TuckerTheSaboteur.actions.AAttackNoIcon()
            {
                damage = GetDmg(s, 2),
                fast = true,
                piercing = (upgrade == Upgrade.B),
            });
            actions.Add(new TuckerTheSaboteur.actions.AAttackNoIcon()
            {
                fromX = cannonX + 1,
                damage = GetDmg(s, 1),
                fast = true,
                piercing = (upgrade == Upgrade.B),
            });

            if (upgrade == Upgrade.A)
            {
                actions.Add(new TuckerTheSaboteur.actions.AAttackNoIcon()
                {
                    fromX = cannonX + 2,
                    damage = GetDmg(s, 1),
                    fast = true,
                    piercing = true,
                });
            }

            int numAttacks = actions.Count;

            foreach (var action in actions)
            {
                var aattack = action as AAttack;
                int offset = (int)(aattack.fromX - cannonX);

                Spr offsetSprite = offset > 0
                    ? (Spr)MainManifest.sprites["icons/Offset_Shot_Right"].Id
                    : (Spr)MainManifest.sprites["icons/Offset_Shot_Left"].Id;


                Icon attackIcon = ABluntAttack.DoWeHaveCannonsThough(s)
                    ? new Icon(Enum.Parse<Spr>(aattack.piercing ? "icons_attackPiercing" : "icons_attack"), aattack.damage, Colors.redd)
                    : new Icon(Enum.Parse<Spr>(aattack.piercing ? "icons_attackPiercingFail" : "icons_attackFail"), aattack.damage, Colors.attackFail);

                actions.Add(new TuckerTheSaboteur.actions.ATooltipDummy()
                {
                    tooltips = new() { }, // eventually put the tooltip for offset attacks here
                    icons = new()
                    {
                        new Icon(offsetSprite, offset, Colors.redd),
                        attackIcon
                    }
                });
            }

            for(int i = 0; i < numAttacks; i++) 
            {
                actions.Add(new ADummyAction());
            }

            return actions;
        }
        public override CardData GetData(State state)
        {
            return new()
            {
                cost = 2
            };
        }
    }
}
