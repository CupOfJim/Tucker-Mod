using CobaltCoreModding.Definitions.ExternalItems;
using FSPRO;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur;

namespace TuckerTheSaboteur.actions
{
    public class AReplay : CardAction
    {
        public override List<Tooltip> GetTooltips(State s)
        {
            return new List<Tooltip>
            {
                new TTGlossary(MainManifest.glossary["AReplay"].Head, null)
            };
        }

        public Card card { get; set; }

        public override void Begin(G g, State s, Combat c)
        {
            foreach (CardAction item in card.GetActionsOverridden(s, c))
            {
                if (!(item is AEndTurn))
                {
                    c.Queue(Mutil.DeepCopy(item));
                }
                if (item is AReplay) return;
            }
        }

        public override Icon? GetIcon(State s)
        {
            return new Icon((Spr)MainManifest.sprites["icons/Replay"].Id, null, Colors.textMain);
        }
    }
}
