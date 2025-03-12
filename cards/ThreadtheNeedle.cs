using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckerTheSaboteur.actions;
using static System.Net.Mime.MediaTypeNames;
using Nickel;
using System.Reflection;

namespace TuckerTheSaboteur.cards;

public class ThreadtheNeedle : Card, IRegisterableCard
{
    public static void Register(IModHelper helper) {
        helper.Content.Cards.RegisterCard("ThreadtheNeedle", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Main.Instance.TuckerDeck.Deck,
                rarity = Rarity.rare,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Art = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/cards/Thread_the_Needle.png")).Sprite,
            Name = Main.Instance.AnyLocalizations.Bind(["card", "ThreadtheNeedle", "name"]).Localize
        });
    }

    public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
        Upgrade.B => [
            new AAttack {
                damage = GetDmg(s, 0),
                status = Status.tempShield,
                statusAmount = 3,
                fast = true,
            }.ApplyOffset( -1),
            new AAttack {
                damage = GetDmg(s, 3),
                fast = true,
            }
        ],
        _ => [
            new AAttack {
                damage = GetDmg(s, 0),
                status = Status.tempShield,
                statusAmount = 2,
                fast = true,
            }.ApplyOffset( -2),
            new AAttack {
                damage = GetDmg(s, 0),
                status = Status.tempShield,
                statusAmount = 2,
                fast = true,
            }.ApplyOffset( 2),
            new AAttack {
                damage = GetDmg(s, upgrade == Upgrade.A ? 4 : 3),
                fast = true,
            }
        ]
    };
    
    public override CardData GetData(State state) => new() {
        cost = 0,
        flippable = upgrade == Upgrade.B,
        artTint = "ffffaa"
    };
}
