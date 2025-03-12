using FSPRO;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur;

namespace TuckerTheSaboteur.actions;

// Copied from ACorrodeDamage
// Do not put this on a card, unless you want the card to trigger fuel leak instantly
public class AFuelLeakDamage : CardAction
{
    public bool targetPlayer;
    public int amount = 1;

    public override void Begin(G g, State s, Combat c)
    {
        timer *= 2.0;
        Ship ship = targetPlayer ? s.ship : c.otherShip;
        if (ship != null)
        {
            ship.NormalDamage(s, c, amount, null);
            Audio.Play(Event.Status_CorrodeHurt);
            ship.PulseStatus(Main.Instance.FuelLeakStatus.Status);
        }
    }
}