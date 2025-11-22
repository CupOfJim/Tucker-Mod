using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Nickel;

namespace TuckerTheSaboteur.cards;

public class MutualGain : Card, IRegisterableCard
{
    public static void Register(IModHelper helper) {
        helper.Content.Cards.RegisterCard("MutualGain", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Main.Instance.TuckerDeck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Art = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/cards/Mutual_Gain.png")).Sprite,
            Name = Main.Instance.AnyLocalizations.Bind(["card", "MutualGain", "name"]).Localize
        });
    }

    public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
        Upgrade.B => [
            new AStatus {
                status = Status.maxShield,
                targetPlayer = false,
                statusAmount = 1
            },
            new AStatus {
                status = Status.shield,
                targetPlayer = false,
                statusAmount = 3
            },
            new AStatus {
                status = Status.maxShield,
                targetPlayer = true,
                statusAmount = 1
            },
            new AStatus {
                status = Status.shield,
                targetPlayer = true,
                statusAmount = 3
            },
        ],
        _ => [
            new AStatus {
                status = Status.shield,
                targetPlayer = false,
                statusAmount = 3
            },
            new AStatus {
                status = Status.shield,
                targetPlayer = true,
                statusAmount = 3
            },
        ]
    };

    public override CardData GetData(State state) => new() {
        cost = upgrade == Upgrade.A ? 0 : 1,
        artTint = "ffffaa"
    };
}