using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Nickel;

namespace TuckerTheSaboteur.cards;

public class QuantumCannon : Card, IRegisterableCard
{
    public static void Register(IModHelper helper) {
        helper.Content.Cards.RegisterCard("QuantumCannon", new()
        {
            CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
            Meta = new()
            {
                deck = Main.Instance.TuckerDeck.Deck,
                rarity = Rarity.common,
                upgradesTo = [Upgrade.A, Upgrade.B]
            },
            Art = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/cards/Quantum_Cannon.png")).Sprite,
            Name = Main.Instance.AnyLocalizations.Bind(["card", "QuantumCannon", "name"]).Localize
        });
    }

    public override List<CardAction> GetActions(State s, Combat c) => [
        new AAttack
        {
            damage = GetDmg(s, upgrade == Upgrade.A ? 2 : 1),
            piercing = true,
            status = upgrade == Upgrade.B ? Status.shield : null,
            statusAmount = 2
        }
    ];

    public override CardData GetData(State state) => new()
    {
        cost = 1,
        artTint = "ffffaa"
    };
}