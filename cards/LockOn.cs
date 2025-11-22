using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Nickel;

namespace TuckerTheSaboteur.cards;

public class Lockon : Card, IRegisterableCard
{
    public static void Register(IModHelper helper) {
        helper.Content.Cards.RegisterCard("Lockon", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Main.Instance.TuckerDeck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Art = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/cards/Lock-on.png")).Sprite,
            Name = Main.Instance.AnyLocalizations.Bind(["card", "Lockon", "name"]).Localize
        });
    }

    public override List<CardAction> GetActions(State s, Combat c) => [
        new AAttack {
            damage = GetDmg(s, upgrade == Upgrade.A ? 3 : 2),
            stunEnemy = true
        },
        new AStatus {
            status = upgrade == Upgrade.B ? Status.engineStall : Status.lockdown,
            statusAmount = 2,
            targetPlayer = true
        }
    ];

    public override CardData GetData(State state) => new()
    {
        cost = 1,
        artTint = "ffffaa"
    };
}