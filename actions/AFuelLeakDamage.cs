using FSPRO;
using Microsoft.Extensions.Logging;
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
    public class AFuelLeakDamage : CardAction
    {
        public bool targetPlayer;

        public override void Begin(G g, State s, Combat c)
        {
            timer *= 2.0;
            Ship ship = (targetPlayer ? s.ship : c.otherShip);
            if (ship != null)
            {
                var leak = ship.Get((Status)MainManifest.statuses["fuel_leak"].Id);
                MainManifest.Instance.Logger.LogInformation("Leacakge: " + leak);
                ship.NormalDamage(s, c, leak, null);
                Audio.Play(Event.Status_CorrodeHurt);
                ship.PulseStatus((Status)MainManifest.statuses["fuel_leak"].Id);
            }
        }
    }
}
