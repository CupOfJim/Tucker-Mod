using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuckerMod.actions
{
    public class ANoIconWrapper : CardAction
    {
        public CardAction action;

        public override Icon? GetIcon(State s)
        {
            return null;
        }

        public override void Begin(G g, State s, Combat c)
        {
            base.timer = 0;
            c.QueueImmediate(action);
        }
    }
}
