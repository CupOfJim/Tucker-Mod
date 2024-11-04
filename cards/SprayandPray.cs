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

[CardMeta(rarity = Rarity.uncommon, upgradesTo = new[] { Upgrade.A, Upgrade.B })]
public class SprayandPray : Card, IRegisterableCard
{
    public static void Register(IModHelper helper) {
        helper.Content.Cards.RegisterCard("SprayandPray", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Main.Instance.TuckerDeck.Deck,
                rarity = Rarity.uncommon,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Art = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/cards/Spray_and_Pray.png")).Sprite,
            Name = Main.Instance.AnyLocalizations.Bind(["card", "SprayandPray", "name"]).Localize
        });
    }

    public override List<CardAction> GetActions(State s, Combat c) => upgrade switch {
        Upgrade.A => [
            new ABluntAttack {
                damage = GetDmg(s, 1),
                fast = true,
            }.ApplyOffset(s, -2),
            new AAttack {
                damage = GetDmg(s, 1),
                fast = true
            }.ApplyOffset(s, -1),
            new AAttack {
                damage = GetDmg(s, 2),
                fast = true
            },
            new AAttack {
                damage = GetDmg(s, 1),
                fast = true
            }.ApplyOffset(s, 1),
            new ABluntAttack {
                damage = GetDmg(s, 1),
                fast = true,
            }.ApplyOffset(s, 2),
        ],
        _ => [
            new AAttack {
                damage = GetDmg(s, 1),
                fast = true,
                piercing = upgrade == Upgrade.B
            }.ApplyOffset(s, -1),
            new AAttack {
                damage = GetDmg(s, 2),
                fast = true,
                piercing = upgrade == Upgrade.B
            },
            new AAttack {
                damage = GetDmg(s, 1),
                fast = true,
                piercing = upgrade == Upgrade.B
            }.ApplyOffset(s, 1),
        ]
    };
    
    public override CardData GetData(State state) => new() {
        cost = 2,
        artTint = "ffffaa"
    };
}