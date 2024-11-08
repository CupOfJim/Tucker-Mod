using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Nickel;

namespace TuckerTheSaboteur.cards;

public class Lockpick : Card, IRegisterableCard
{
    public static void Register(IModHelper helper) {
        helper.Content.Cards.RegisterCard("Lockpick", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Main.Instance.TuckerDeck.Deck,
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Art = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/cards/Lockpick.png")).Sprite,
            Name = Main.Instance.AnyLocalizations.Bind(["card", "Lockpick", "name"]).Localize
        });
    }

    public override List<CardAction> GetActions(State s, Combat c) => [
        new AStatus {
            status = Status.lockdown,
            statusAmount = -2,
            targetPlayer = true
        },
        new AStatus {
            status = Status.engineStall,
            statusAmount = -2,
            targetPlayer = true
        },
        new AStatus {
            status = upgrade == Upgrade.B ? Status.shield : Status.tempShield,
            statusAmount = 1,
            targetPlayer = true
        }
    ];

    public override CardData GetData(State state) => new() {
        cost = 0,
        retain = upgrade == Upgrade.B,
        artTint = "ffffaa"
    };
}