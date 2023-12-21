using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuckerTheSaboteur.actions
{
    public class AAttackNoIcon : AAttack
    {
        public override Icon? GetIcon(State s)
        {
            return null;
        }
    }
}
