using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur.actions;
using Nickel;

namespace TuckerTheSaboteur.cards;

public class CounterfeitSwap : Card, IRegisterableCard
{
    public static void Register(IModHelper helper) {
        helper.Content.Cards.RegisterCard("CounterfeitSwap", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Main.Instance.TuckerDeck.Deck,
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Art = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/cards/Counterfeit_Swap.png")).Sprite,
            Name = Main.Instance.AnyLocalizations.Bind(["card", "CounterfeitSwap", "name"]).Localize
        });
    }

    public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
        Upgrade.A => [
            new AShieldSteal { amount = 4 },
            new AStatus {
                status = Status.tempShield,
                statusAmount = 4,
                targetPlayer = false
            }
        ],
        Upgrade.B => [
            new AStatus {
                status = Status.tempShield,
                statusAmount = 3,
                targetPlayer = false
            },
            new AShieldSteal { amount = 3 }
        ],
        _ => [
            new AShieldSteal { amount = 3 },
            new AStatus {
                status = Status.tempShield,
                statusAmount = 3,
                targetPlayer = false
            }
        ]
    };

    public override CardData GetData(State state) => new() {
        cost = 1,
        artTint = "ffffaa"
    };
}
