using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur.actions;
using Nickel;
using System.Reflection;

namespace TuckerTheSaboteur.cards;

public class Cripple : Card, IRegisterableCard
{
    public static void Register(IModHelper helper) {
        helper.Content.Cards.RegisterCard("Cripple", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Main.Instance.TuckerDeck.Deck,
                rarity = Rarity.rare,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Art = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/cards/Cripple.png")).Sprite,
            Name = Main.Instance.AnyLocalizations.Bind(["card", "Cripple", "name"]).Localize
        });
    }

    public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
        Upgrade.B => [
            new ABluntAttack {
                damage = GetDmg(s, 2),
                status = Main.Instance.FuelLeakStatus.Status,
                statusAmount = 1,
            },
            new AAttack {
                damage = GetDmg(s, 2),
                status = Main.Instance.FuelLeakStatus.Status,
                statusAmount = 1,
            }
        ],
        _ => [
            new ABluntAttack {
                damage = GetDmg(s, 4),
                status = Main.Instance.FuelLeakStatus.Status,
                statusAmount = 1,
            }
        ]
    };

    public override CardData GetData(State state) => new() {
        cost = upgrade switch {
            Upgrade.A => 2,
            Upgrade.B => 4,
            _ => 3,
        },
        artTint = "ffffaa"
    };
}