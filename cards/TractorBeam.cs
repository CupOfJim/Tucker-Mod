using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur.actions;
using Nickel;
using System.Reflection;

namespace TuckerTheSaboteur.cards;

public class TractorBeam : Card, IRegisterableCard
{
    public static void Register(IModHelper helper) {
        helper.Content.Cards.RegisterCard("TractorBeam", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Main.Instance.TuckerDeck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Art = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/cards/Tractor_Beam.png")).Sprite,
            Name = Main.Instance.AnyLocalizations.Bind(["card", "TractorBeam", "name"]).Localize
        });
    }

    public override List<CardAction> GetActions(State s, Combat c) => [
        new AShieldSteal {
            amount = 2
        }
    ];
    public override CardData GetData(State state) => new() {
        cost = 0,
        exhaust = upgrade != Upgrade.A,
        buoyant = upgrade == Upgrade.B,
        artTint = "ffffaa"
    };
}
