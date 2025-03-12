using System.Collections.Generic;
using System.Reflection;
using Nickel;

namespace TuckerTheSaboteur.cards;

public class Counterattack : Card, IRegisterableCard
{
    public static void Register(IModHelper helper) {
        helper.Content.Cards.RegisterCard("Counterattack", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Main.Instance.TuckerDeck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B],
                dontOffer = true
            },
            Art = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/cards/Counter-attack.png")).Sprite,
            Name = Main.Instance.AnyLocalizations.Bind(["card", "Counterattack", "name"]).Localize
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c) => [
        new AAttack {
            damage = GetDmg(s, upgrade == Upgrade.A ? 3 : 2),
            piercing = true,
        }
    ];

    public override CardData GetData(State state)
    {
        return new()
        {
            cost = 0,
            temporary = true,
            exhaust = upgrade != Upgrade.B,
            artTint = "ffffaa"
        };
    }
}
