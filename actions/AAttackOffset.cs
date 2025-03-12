using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuckerTheSaboteur.actions;

public class AAttackOffset : AAttack
{
    public int offset;

	public override void Begin(G g, State s, Combat c)
	{
        int cannonX = s.ship.parts.FindIndex((Part p) => p.type == PType.cannon && p.active);
        fromX = cannonX + offset;
		base.Begin(g, s, c);
	}
}
