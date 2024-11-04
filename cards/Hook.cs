using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur.actions;
using Nickel;
using System.Reflection;

namespace TuckerTheSaboteur.cards;

public class Hook : Card, IRegisterableCard
{
    public static void Register(IModHelper helper) {
        helper.Content.Cards.RegisterCard("Hook", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Main.Instance.TuckerDeck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Art = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/cards/Hook.png")).Sprite,
            Name = Main.Instance.AnyLocalizations.Bind(["card", "Hook", "name"]).Localize
        });
    }
    
    public override List<CardAction> GetActions(State s, Combat c) => [
        new AAttack {
            damage = GetDmg(s, 2),
            moveEnemy = 2,
        }.ApplyOffset(s, upgrade == Upgrade.B ? -1 : -3)
    ];
    public override CardData GetData(State state)
    {
        return new()
        {
            cost = 1,
            flippable = upgrade == Upgrade.A ? true : false,
            artTint = "ffffaa"
        };
    }
}