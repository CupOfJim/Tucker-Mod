using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Nickel;
using TuckerTheSaboteur.Actions;

namespace TuckerTheSaboteur.cards;

public class NavOverride : Card, IRegisterableCard
{
    public static void Register(IModHelper helper) {
        helper.Content.Cards.RegisterCard("NavOverride", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Main.Instance.TuckerDeck.Deck,
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Art = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/cards/Nav_Override.png")).Sprite,
            Name = Main.Instance.AnyLocalizations.Bind(["card", "NavOverride", "name"]).Localize
        });
    }

    public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
        Upgrade.B => [
            new AAddCard {
                card = new Misdirection(),
                amount = 2,
                destination = CardDestination.Hand
            }
        ],
        _ => [
            new AMoveImproved {
                dir = -1,
                targetPlayer = false
            },
            new AAddCard {
                card = new Misdirection(),
                destination = CardDestination.Hand
            }
        ]
    };

    public override CardData GetData(State state) => new() {
        cost = 1,
        flippable = upgrade != Upgrade.B,
        description = Main.Instance.Localizations.Localize(["card", "NavOverride", "description", upgrade.ToString()],
            new {
                Direction = Main.Instance.Localizations.Localize(["card", "NavOverride", flipped ? "right" : "left"]) 
            }),
        artTint = "ffffaa"
    };
}