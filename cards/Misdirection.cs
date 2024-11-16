using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Nickel;
using TuckerTheSaboteur.Actions;

namespace TuckerTheSaboteur.cards;

public class Misdirection : Card, IRegisterableCard
{
    public static void Register(IModHelper helper) {
        helper.Content.Cards.RegisterCard("Misdirection", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Main.Instance.TuckerDeck.Deck,
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B],
                dontOffer = true
            },
            Art = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/cards/Misdirection.png")).Sprite,
            Name = Main.Instance.AnyLocalizations.Bind(["card", "Misdirection", "name"]).Localize
        });
    }

    public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
        Upgrade.B => [
            new AMoveImproved {
                dir = -1,
                targetPlayer = false
            },
            new ADrawCard
            {
                count = 1
            }
        ],
        _ => [
            new AMoveImproved {
                dir = upgrade == Upgrade.A ? -2 : -1,
                targetPlayer = false
            },
        ]
    };

    public override CardData GetData(State state) => new() {
        cost = 0,
        temporary = true,
        exhaust = true,
        retain = true,
        flippable = true,
        artTint = "ffffaa"
    };
}