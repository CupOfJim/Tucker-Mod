using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Nickel;

namespace TuckerTheSaboteur.cards;

public class FortressMode : Card, IRegisterableCard
{
    public static void Register(IModHelper helper) {
        helper.Content.Cards.RegisterCard("FortressMode", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Main.Instance.TuckerDeck.Deck,
                rarity = Rarity.rare,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Art = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/cards/Fortress_Mode.png")).Sprite,
            Name = Main.Instance.AnyLocalizations.Bind(["card", "FortressMode", "name"]).Localize
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
        Upgrade.B => [
            new AStatus {
                status = Main.Instance.BufferStatus.Status,
                statusAmount = 3,
                targetPlayer = true
            },
            new AStatus {
                status = Status.engineStall,
                statusAmount = 3,
                targetPlayer = true
            }
        ],
        _ => [
            new AStatus {
                status = Main.Instance.BufferStatus.Status,
                statusAmount = upgrade == Upgrade.A ? 4 : 3,
                targetPlayer = true
            },
            new AStatus {
                status = Status.lockdown,
                statusAmount = 3,
                targetPlayer = true
            }
        ]
    };

    public override CardData GetData(State state) => new() {
        cost = 2,
        artTint = "ffffaa"
    };
}
