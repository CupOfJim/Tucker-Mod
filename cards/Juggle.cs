﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuckermod.actions;

namespace TuckerTheSaboteur.cards
{
    [CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
    public class Juggle : Card
    {
        public override List<CardAction> GetActions(State s, Combat c)
        {
            Upgrade upgrade = base.upgrade;
            switch (upgrade)
            {
                case Upgrade.None:
                    return new List<CardAction> ()
                    {
                        new AAttack ()
                        {
                            fromX = -1,
                            damage = GetDmg(s, 0),
                            fast = true,
                            moveEnemy = 2,
                        },
                        new AAttack ()
                        {
                            fromX = 1,
                            damage = GetDmg(s, 0),
                            fast = true,
                            moveEnemy = -2,
                        },
                        new AAttack ()
                        {
                            fromX = -1,
                            damage = GetDmg(s, 3),
                            fast = true,
                        }
                    };
                case Upgrade.A:
                    return new List<CardAction> ()
                    {
                        new AAttack ()
                        {
                            fromX = -1,
                            damage = GetDmg(s, 0),
                            fast = true,
                            moveEnemy = 2,
                        },
                        new AAttack ()
                        {
                            fromX = 1,
                            damage = GetDmg(s, 0),
                            fast = true,
                            moveEnemy = -2,
                        },
                        new AReplay(),
                        new AAttack ()
                        {
                            fromX = -1,
                            damage = GetDmg(s, 4),
                            fast = true,
                        }
                    };
                case Upgrade.B:
                    return new List<CardAction> ()
                    {
                        new AAttack ()
                        {
                            fromX = -2,
                            damage = GetDmg(s, 0),
                            fast = true,
                            moveEnemy = 2,
                        },
                        new AAttack ()
                        {

                            damage = GetDmg(s, 0),
                            fast = true,
                            moveEnemy = -2,
                        },
                        new AAttack ()
                        {
                            fromX = 2,
                            damage = GetDmg(s, 3),
                            fast = true,
                        }
                    };
            }

            throw new Exception("Juggle was upgraded to something that doesn't exist.");
        }
        public override CardData GetData(State state)
        {
            return new()
            {
                cost = 1
            };
        }
    }
}
