using FSPRO;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;
using TuckerMod.tuckerArtifacts.Duos;

namespace TuckerTheSaboteur.actions;

public class AShieldSteal : CardAction
{
    public int amount;

    public override void Begin(G g, State s, Combat c)
    {
        ToolsOfTheTrade? tools = s.EnumerateAllArtifacts().OfType<ToolsOfTheTrade>().FirstOrDefault();
        int enemyShield = c.otherShip.Get(Status.shield);
        int steal = Math.Min(enemyShield, amount);
        Audio.Play(Event.Status_PowerUp);
        c.otherShip.Set(Status.shield, enemyShield - steal);

        if (steal > 0)
            c.QueueImmediate(new AStatus {
                status = Status.shield,
                statusAmount = steal + (tools != null ? 1 : 0),
                targetPlayer = true,
                artifactPulse = tools?.Key()
            });

        int enemyTempShield = c.otherShip.Get(Status.tempShield);
        int tempSteal = Math.Min(enemyTempShield, amount - steal);
        c.otherShip.Set(Status.tempShield, enemyTempShield - tempSteal);

        if (tempSteal > 0) {
            c.QueueImmediate(new AStatus {
                status = Status.tempShield,
                statusAmount = tempSteal + (tools != null ? 1 : 0),
                targetPlayer = true,
                artifactPulse = tools?.Key()
            });
        }
    }

    private Spr GetSpr() => Main.Instance.ShieldStealSprite;

    public override Icon? GetIcon(State s)
    {
        return new Icon(GetSpr(), amount, Colors.redd);
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        return [
			new GlossaryTooltip("action.shieldSteal") {
                TitleColor = Colors.action,
                Icon = GetSpr(),
                Title = Main.Instance.Localizations.Localize(["action", "shieldSteal", "name"]),
                Description = Main.Instance.Localizations.Localize(["action", "shieldSteal", "description"], new { Amount = amount }),
            },
            .. StatusMeta.GetTooltips(Status.shield, amount),
            .. StatusMeta.GetTooltips(Status.tempShield, amount)
        ];
    }
}