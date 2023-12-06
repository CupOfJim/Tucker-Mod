﻿using PhilipTheMechanic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace TuckerMod.actions
{
    public class AShieldSteal : CardAction
    {
        public int amount;

        public override void Begin(G g, State s, Combat c)
        {
            int enemyShield = c.otherShip.Get(Enum.Parse<Status>("shield"));
            int steal = Math.Min(enemyShield, amount);
            c.otherShip.Set(Enum.Parse<Status>("shield"), enemyShield - steal);

            c.QueueImmediate(new AStatus()
            {
                status = Enum.Parse<Status>("shield"),
                statusAmount = amount,
                targetPlayer = true
            });
        }

        public override Icon? GetIcon(State s)
        {
            return new Icon((Spr)MainManifest.sprites["icons/Shield_Steal"].Id, amount, Colors.redd);
        }
    }
}