// AMove
using System;
using System.Collections.Generic;

namespace TuckerTheSaboteur.Actions
{
	public class AMoveImproved : AMove
	{
		public override List<Tooltip> GetTooltips(State s)
		{
			if (!targetPlayer) {
				return [
					new CustomTTGlossary(
						CustomTTGlossary.GlossaryType.action,
						() => dir < 0 ? StableSpr.icons_moveLeftEnemy : StableSpr.icons_moveRightEnemy,
						() => Main.Instance.Localizations.Localize(["action", "enemyMove", "name", dir < 0 ? "left" : "right"], new {Amount = Math.Abs(dir)}),
						() => Main.Instance.Localizations.Localize(["action", "enemyMove", "description", dir < 0 ? "left" : "right"], new {Amount = Math.Abs(dir)}),
						key: "action.moveEnemy")
					];
			} else {
				return base.GetTooltips(s);
			}
		}
	}
}