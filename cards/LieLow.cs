using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Nickel;

namespace TuckerTheSaboteur.cards;

public class LieLow : Card, IRegisterableCard
{
    public static void Register(IModHelper helper) {
        helper.Content.Cards.RegisterCard("LieLow", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Main.Instance.TuckerDeck.Deck,
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Art = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/cards/Lie_Low.png")).Sprite,
            Name = Main.Instance.AnyLocalizations.Bind(["card", "LieLow", "name"]).Localize
        });
    }

    public override List<CardAction> GetActions(State s, Combat c) => [
        new AStatus {
            status = Status.maxShield,
            targetPlayer = true,
            statusAmount = 1,
            mode = AStatusMode.Add,
        },
        new AStatus {
            status = Status.shield,
            statusAmount = upgrade == Upgrade.A ? 3 : 2,
            targetPlayer = true
        },
        new AStatus {
            status = Status.lockdown,
            statusAmount = upgrade == Upgrade.B ? 3 : 2,
            targetPlayer = true
        }
    ];

    public override CardData GetData(State state) => new()
    {
        cost = upgrade == Upgrade.B ? 0 : 1,
        artTint = "ffffaa"
    };
}