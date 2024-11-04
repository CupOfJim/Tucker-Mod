using FSPRO;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace TuckerTheSaboteur.actions;

public class AShieldSteal : CardAction
{
    public int amount;

    public override void Begin(G g, State s, Combat c)
    {
        int enemyShield = c.otherShip.Get(Status.shield);
        int steal = Math.Min(enemyShield, amount);
        Audio.Play(Event.Status_PowerUp);
        c.otherShip.Set(Status.shield, enemyShield - steal);

        c.QueueImmediate(new AStatus {
            status = Status.shield,
            statusAmount = steal,
            targetPlayer = true
        });
        int enemyTempShield = c.otherShip.Get(Status.tempShield);
        int tempSteal = Math.Min(enemyTempShield, amount - steal);
        c.otherShip.Set(Status.tempShield, enemyTempShield - tempSteal);

        c.QueueImmediate(new AStatus {
            status = Status.tempShield,
            statusAmount = tempSteal,
            targetPlayer = true
        });
    }

    private Spr GetSpr() => Main.Instance.ShieldStealSprite;

    public override Icon? GetIcon(State s)
    {
        return new Icon(GetSpr(), amount, Colors.redd);
    }

    public override List<Tooltip> GetTooltips(State s)
    {
        return [
			new CustomTTGlossary(
                CustomTTGlossary.GlossaryType.action,
                () => GetSpr(),
                () => Main.Instance.Localizations.Localize(["action", "shieldSteal", "name"]),
                () => Main.Instance.Localizations.Localize(["action", "shieldSteal", "description"], new { Amount = amount }),
                key: "action.shieldSteal"
            ),
            .. StatusMeta.GetTooltips(Status.shield, amount),
            .. StatusMeta.GetTooltips(Status.tempShield, amount)
        ];
    }
}