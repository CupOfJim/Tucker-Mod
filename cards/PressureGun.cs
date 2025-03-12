using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur.actions;
using Nickel;
using System.Reflection;

namespace TuckerTheSaboteur.cards;

public class PressureGun : Card, IRegisterableCard
{
    public static void Register(IModHelper helper) {
        helper.Content.Cards.RegisterCard("PressureGun", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Main.Instance.TuckerDeck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Art = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/cards/Pressure_Gun.png")).Sprite,
            Name = Main.Instance.AnyLocalizations.Bind(["card", "PressureGun", "name"]).Localize
        });
    }

    public override List<CardAction> GetActions(State s, Combat c) => [
        upgrade == Upgrade.B ? 
            new AAttack {
                damage = GetDmg(s, 0),
                moveEnemy = -3,
            } : new ABluntAttack {
                damage = GetDmg(s, 0),
                moveEnemy = -3,
            }
    ];

    public override CardData GetData(State state) => new() {
        cost = 0,
        flippable = upgrade == Upgrade.A,
        artTint = "ffffaa"
    };
}