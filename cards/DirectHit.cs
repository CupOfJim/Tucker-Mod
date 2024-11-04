using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur.actions;
using Nickel;
using System.Reflection;

namespace TuckerTheSaboteur.cards;

public class DirectHit : Card, IRegisterableCard
{
    public static void Register(IModHelper helper) {
        helper.Content.Cards.RegisterCard("DirectHit", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Main.Instance.TuckerDeck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Art = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/cards/Direct_Hit.png")).Sprite,
            Name = Main.Instance.AnyLocalizations.Bind(["card", "DirectHit", "name"]).Localize
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
        Upgrade.B => [
            new AStatus {
                status = Status.shield,
                statusAmount = -2,
                targetPlayer = false,
            },
            new ABluntAttack {
                damage = GetDmg(s, 3),
            }
        ],
        _ => [
            new ABluntAttack {
                damage = GetDmg(s, upgrade == Upgrade.A ? 4 : 3),
            }
        ]
    };

    public override CardData GetData(State state) => new() {
        cost = 1,
        artTint = "ffffaa"
    };
}