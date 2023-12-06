using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuckerTheSaboteur.actions
{
    public class ABluntAttack : AAttack
    {
        public override void Begin(G g, State s, Combat c)
        {
            int enemyShield = c.otherShip.Get(Enum.Parse<Status>("shield")) + c.otherShip.Get(Enum.Parse<Status>("tempShield"));
            if (enemyShield > 0) 
            {
                base.status = null;
                base.statusAmount = 0;
                base.damage = 0;
                base.moveEnemy = 0;
                base.stunEnemy = false;
                base.weaken = false;
                base.brittle = false;
                base.armorize = false;
            }

            base.Begin(g, s, c);
        }

        // Lifted directly from decompiled game code
        // Yes it is called that in the source
        // I love the devs
        private bool DoWeHaveCannonsThough(State s)
        {
            foreach (Part part in s.ship.parts)
            {
                if (part.type == PType.cannon)
                {
                    return true;
                }
            }
            if (s.route is Combat combat)
            {
                foreach (StuffBase value in combat.stuff.Values)
                {
                    if (value is JupiterDrone)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override Icon? GetIcon(State s)
        {
            if (DoWeHaveCannonsThough(s))
            {
                return new Icon((Spr)MainManifest.sprites["icons/Blunt_Attack"].Id, damage, Colors.redd);
            }

            return new Icon((Spr)MainManifest.sprites["icons/Blunt_Attack_Fail"].Id, damage, Colors.redd);
        }
    }
}
