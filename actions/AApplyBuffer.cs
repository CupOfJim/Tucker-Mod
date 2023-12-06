using FSPRO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur;

namespace TuckerTheSaboteur.actions
{
    // Copied from ACorrodeDamage
    // Do not put this on a card, unless you want the card to trigger fuel leak instantly
    public class AApplyBuffer : CardAction
    {
        public bool targetPlayer;

        public override void Begin(G g, State s, Combat c)
        {
            timer *= 2.0;
            Ship ship = (targetPlayer ? s.ship : c.otherShip);
            int bufferAmount = ship.Get((Status)MainManifest.statuses["buffer"].Id);
            if (ship != null)
            {
                c.QueueImmediate(new AStatus()
                {
                    status = Enum.Parse<Status>("tempShield"),
                    targetPlayer = targetPlayer,
                    statusAmount = bufferAmount
                });
                ship.PulseStatus((Status)MainManifest.statuses["buffer"].Id);
                ship.Set((Status)MainManifest.statuses["buffer"].Id, bufferAmount - 1);
            }
        }
    }
}
