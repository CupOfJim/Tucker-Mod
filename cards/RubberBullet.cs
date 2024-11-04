using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur.actions;
using Nickel;
using System.Reflection;

namespace TuckerTheSaboteur.cards;

public class RubberBullet : Card, IRegisterableCard
{
    public static void Register(IModHelper helper) {
        helper.Content.Cards.RegisterCard("RubberBullet", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Main.Instance.TuckerDeck.Deck,
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Art = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/cards/Rubber_Bullet.png")).Sprite,
            Name = Main.Instance.AnyLocalizations.Bind(["card", "RubberBullet", "name"]).Localize
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
                damage = GetDmg(s, 2),
                stunEnemy = true
            }
        ],
        _ => [
            new ABluntAttack {
                damage = GetDmg(s, upgrade == Upgrade.A ? 3 : 2),
                stunEnemy = true
            }
        ]
    };
    
    public override CardData GetData(State state) => new() {
        cost = 1,
        artTint = "ffffaa"
    };
}