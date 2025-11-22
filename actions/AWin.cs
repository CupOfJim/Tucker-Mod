using System.Collections.Generic;

namespace TuckerTheSaboteur.actions;

public class AWin : AStartCombat
{
	public override void Begin(G g, State s, Combat c)
	{
		s.rewardsQueue.Clear();
		s.TryCloseRoute(g, s.route, null);
		s.RemoveAllTempCards();
		s.ChangeRoute(() => s.MakeRunWinRoute());
	}

	public override List<Tooltip> GetTooltips(State s) => [];
}
