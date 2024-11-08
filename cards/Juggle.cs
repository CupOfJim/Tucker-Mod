using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur.actions;
using Nickel;
using System.Reflection;

namespace TuckerTheSaboteur.cards;

public class Juggle : Card, IRegisterableCard
{
    public static void Register(IModHelper helper) {
        helper.Content.Cards.RegisterCard("Juggle", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Main.Instance.TuckerDeck.Deck,
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Art = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/cards/Juggle.png")).Sprite,
            Name = Main.Instance.AnyLocalizations.Bind(["card", "Juggle", "name"]).Localize
        });
    }

    public override List<CardAction> GetActions(State s, Combat c) => [
        new AAttack {
            damage = GetDmg(s, 0),
            fast = true,
            moveEnemy = upgrade == Upgrade.B ? -2 : 2,
        }.ApplyOffset(s, upgrade == Upgrade.B ? 2 : -1),
        new AAttack {
            damage = GetDmg(s, 0),
            fast = true,
            moveEnemy = -2,
        }.ApplyOffset(s, upgrade == Upgrade.B ? 0 : 1),
        new AAttack {
            damage = GetDmg(s, upgrade == Upgrade.A ? 5 : 3),
            fast = true,
        }.ApplyOffset(s, upgrade == Upgrade.B ? -2 : -1)
    ];

    public override CardData GetData(State state) => new() {
        cost = 2,
        artTint = "ffffaa"
    };
}