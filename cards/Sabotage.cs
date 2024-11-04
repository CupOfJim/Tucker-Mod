using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur.actions;
using Nickel;
using System.Reflection;

namespace TuckerTheSaboteur.cards;

public class Sabotage : Card, IRegisterableCard
{
    public static void Register(IModHelper helper) {
        helper.Content.Cards.RegisterCard("Sabotage", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Main.Instance.TuckerDeck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Art = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/cards/Sabotage.png")).Sprite,
            Name = Main.Instance.AnyLocalizations.Bind(["card", "Sabotage", "name"]).Localize
        });
    }

    public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
        Upgrade.A => [
            new AShieldSteal {
                amount = 2
            },
            new AStatus {
                status = Status.shield,
                statusAmount = -1,
                targetPlayer = false
            }
        ],
        Upgrade.B => [
            new AShieldSteal {
                amount = 2
            },
            new AStatus {
                status = Status.shield,
                statusAmount = 1,
                targetPlayer = true
            }
        ],
        _ => [
            new AShieldSteal {
                amount = 2
            },
        ]
    };

    public override CardData GetData(State state) => new() {
        cost = 1,
        artTint = "ffffaa"
    };
}