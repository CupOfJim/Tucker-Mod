using System;
using System.Collections.Generic;
using TuckerTheSaboteur.actions;
using Nickel;
using System.Reflection;

namespace TuckerTheSaboteur.cards;

public class KnowThyEnemy : Card, IRegisterableCard
{
    static Spr topSprite;
    static Spr bottomSprite;
    
    public static void Register(IModHelper helper) {
        topSprite = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/cards/MiningDrill_Top.png")).Sprite;
        bottomSprite = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/cards/MiningDrill_Bottom.png")).Sprite;

        helper.Content.Cards.RegisterCard("KnowThyEnemy", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Main.Instance.TuckerDeck.Deck,
                rarity = Rarity.rare,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Art = topSprite,
            Name = Main.Instance.AnyLocalizations.Bind(["card", "KnowThyEnemy", "name"]).Localize
        });
    }

    public override List<CardAction> GetActions(State s, Combat c) => [
        new AAttack {
            damage = GetDmg(s, upgrade == Upgrade.A ? 3 : 2),
            piercing = true,
            disabled = flipped,
        },
        upgrade == Upgrade.A ? new AShieldSteal {
            amount = 2,
            disabled = flipped
        } : new AStatus {
            status = Status.shield,
            statusAmount = -2,
            targetPlayer = false,
            disabled = flipped,
        },
        new ADummyAction(),
        new ABluntAttack {
            damage = GetDmg(s, upgrade == Upgrade.B ? 6 : 5),
            disabled = !flipped,
        }
    ];
    
    public override CardData GetData(State state) => new()
    {
        cost = 2,
        floppable = true,
        art = flipped ? bottomSprite : topSprite,
        artTint = "ffffff"
    };
}