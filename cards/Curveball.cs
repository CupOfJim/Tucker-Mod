using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur.actions;
using Nickel;
using System.Reflection;

namespace TuckerTheSaboteur.cards;

public class Curveball : Card, IRegisterableCard
{
    public static void Register(IModHelper helper) {
        helper.Content.Cards.RegisterCard("Curveball", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Main.Instance.TuckerDeck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Art = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/cards/Curveball.png")).Sprite,
            Name = Main.Instance.AnyLocalizations.Bind(["card", "Curveball", "name"]).Localize
        });
    }

    public override List<CardAction> GetActions(State s, Combat c) => [
        new AAttack {
            damage = GetDmg(s, 2)
        }.ApplyOffset( upgrade == Upgrade.B ? -4 : -2)
    ];

    public override CardData GetData(State state) => new() {
        cost = upgrade == Upgrade.B ? 0 : 1,
        flippable = upgrade == Upgrade.A,
        artTint = "ffffaa"
    };
}