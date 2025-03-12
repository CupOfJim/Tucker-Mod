using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using TuckerTheSaboteur.actions;
using Nickel;
using System.Reflection;

namespace TuckerTheSaboteur.cards;

public class PsychOut : Card, IRegisterableCard
{
    public static void Register(IModHelper helper) {
        helper.Content.Cards.RegisterCard("PsychOut", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Main.Instance.TuckerDeck.Deck,
                rarity = Rarity.rare,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Art = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/cards/Psych_Out.png")).Sprite,
            Name = Main.Instance.AnyLocalizations.Bind(["card", "PsychOut", "name"]).Localize
        });
    }

    public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
        Upgrade.B => [
            new AStatus {
                status = Status.autododgeLeft,
                statusAmount = 2,
                targetPlayer = false
            },
            new AAttack {
                damage = GetDmg(s, 1),
                fast = true
            },
            new AAttack {
                damage = GetDmg(s, 1),
                fast = true,
            }.ApplyOffset( -2)
        ],
        _ => [
            new AStatus {
                status = Status.autododgeLeft,
                statusAmount = 1,
                targetPlayer = false
            },
            new AAttack {
                damage = GetDmg(s, 1)
            }
        ]
    };

    public override CardData GetData(State state) => new() {
        cost = upgrade == Upgrade.A ? 1 : 2,
        exhaust = true,
        artTint = "ffffaa"
    };
}