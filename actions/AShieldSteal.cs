using FSPRO;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace TuckerTheSaboteur.actions
{
    public class AShieldSteal : CardAction
    {
        public int amount;

        public override void Begin(G g, State s, Combat c)
        {
            int enemyShield = c.otherShip.Get(Enum.Parse<Status>("shield"));
            int steal = Math.Min(enemyShield, amount);
            Audio.Play(Event.Status_PowerUp);
            c.otherShip.Set(Enum.Parse<Status>("shield"), enemyShield - steal);

            c.QueueImmediate(new AStatus()
            {
                status = Enum.Parse<Status>("shield"),
                statusAmount = steal,
                targetPlayer = true
            });
            int enemyTempShield = c.otherShip.Get(Enum.Parse<Status>("tempShield"));
            int tempSteal = Math.Min(enemyTempShield, amount - steal);
            c.otherShip.Set(Enum.Parse<Status>("tempShield"), enemyTempShield - tempSteal);

            c.QueueImmediate(new AStatus()
            {
                status = Enum.Parse<Status>("tempShield"),
                statusAmount = tempSteal,
                targetPlayer = true
            });
        }

        private Spr GetSpr() { return (Spr)MainManifest.sprites["icons/Shield_Steal"].Id; }

        public override Icon? GetIcon(State s)
        {
            return new Icon(GetSpr(), amount, Colors.redd);
        }

        public override List<Tooltip> GetTooltips(State s)
        {
            var y = 0;
            var x = 1 / y;
            return new()
            {
                new Shockah.Kokoro.CustomTTGlossary
                (
                    Shockah.Kokoro.CustomTTGlossary.GlossaryType.action,
                    () => GetSpr(),
                    () => "Shield Steal",
                    () => "Removes up to {0} shield and temp shield from the enemy (preferring shield) and grants you an equal amount of temp shield.",
                    new List<Func<object>>()
                    {
                        () => ""+amount,
                    }
                )
            };
        }
    }
}