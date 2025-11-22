
using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Nickel;

namespace TuckerTheSaboteur;

[HarmonyPatch]
public static partial class BirthdaySecretController
{
	private static ICardEntry? uraniumRound = null;
	private static ICardEntry? blackMarketParts = null;
	private static void RegisterArtifacts(IModHelper helper) {
		if (philipDeck == null) return;

        helper.Content.Artifacts.RegisterArtifact("UraniumIdea", new()
		{
			ArtifactType = typeof(UraniumIdea),
			Meta = new()
			{
				owner = philipDeck!.Value,
				pools = [ArtifactPool.EventOnly]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/stuff/Idea.png")).Sprite,
			Name = Main.Instance.AnyLocalizations.Bind(["artifact", "Idea", "name"]).Localize,
			Description = Main.Instance.AnyLocalizations.Bind(["artifact", "Idea", "description"]).Localize
		});
        helper.Content.Artifacts.RegisterArtifact("UraniumHonor", new()
		{
			ArtifactType = typeof(UraniumHonor),
			Meta = new()
			{
				owner = philipDeck!.Value,
				pools = [ArtifactPool.EventOnly]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/stuff/Honor.png")).Sprite,
			Name = Main.Instance.AnyLocalizations.Bind(["artifact", "Honor", "name"]).Localize,
			Description = Main.Instance.AnyLocalizations.Bind(["artifact", "Honor", "description"]).Localize
		});
        helper.Content.Artifacts.RegisterArtifact("UraniumTable", new()
		{
			ArtifactType = typeof(UraniumTable),
			Meta = new()
			{
				owner = philipDeck!.Value,
				pools = [ArtifactPool.EventOnly]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/stuff/Table.png")).Sprite,
			Name = Main.Instance.AnyLocalizations.Bind(["artifact", "Table", "name"]).Localize,
			Description = Main.Instance.AnyLocalizations.Bind(["artifact", "Table", "description"]).Localize
		});
        helper.Content.Artifacts.RegisterArtifact("UraniumRoundTable", new()
		{
			ArtifactType = typeof(UraniumRoundTable),
			Meta = new()
			{
				owner = philipDeck!.Value,
				pools = [ArtifactPool.EventOnly]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/stuff/RoundTable.png")).Sprite,
			Name = Main.Instance.AnyLocalizations.Bind(["artifact", "RoundTable", "name"]).Localize,
			Description = Main.Instance.AnyLocalizations.Bind(["artifact", "RoundTable", "description"]).Localize
		});

		helper.Content.Cards.OnGetFinalDynamicCardTraitOverrides += (_, args) => {
			if (args.State.EnumerateAllArtifacts().OfType<UraniumRoundTable>().Any()) {
				if (args.Card.GetMeta().deck != philipDeck.Value) {
					args.SetOverride(helper.Content.Cards.ExhaustCardTrait, true);
				}
			}
		};
	}

	private static void AddUranium(State state, int amount = 1) {
		Card card = (Card)Activator.CreateInstance(uraniumRound!.Configuration.CardType)!;
		card.upgrade = Upgrade.A;
		Main.Instance.Helper.Content.Cards.SetCardTraitOverride(state, card, Main.Instance.Helper.Content.Cards.TemporaryCardTrait, false, true);
		state.GetCurrentQueue().Queue(new AAddCard {
			card = card,
			amount = amount
		});
	}

    public class UraniumIdea : Artifact {
		private static Card? uraniumRound;
		public override void OnReceiveArtifact(State state) =>
			AddUranium(state);

		public override List<Tooltip>? GetExtraTooltips()
		{
			if (uraniumRound == null) {
				uraniumRound = (Card)Activator.CreateInstance(BirthdaySecretController.uraniumRound!.Configuration.CardType)!;
				uraniumRound.upgrade = Upgrade.A;
			}
			return [new TTCard {
				card = uraniumRound,
			}];
		}
	
    }

    public class UraniumHonor : Artifact {
		private static Card? uraniumRound;
		public override void OnReceiveArtifact(State state) =>
			AddUranium(state);

		public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
		{
			if (card.GetType() == BirthdaySecretController.uraniumRound!.Configuration.CardType) {
				combat.Queue(new ADrawCard {
					count = 1,
					artifactPulse = Key()
				});
			}
		}

		public override List<Tooltip>? GetExtraTooltips()
		{
			if (uraniumRound == null) {
				uraniumRound = (Card)Activator.CreateInstance(BirthdaySecretController.uraniumRound!.Configuration.CardType)!;
				uraniumRound.upgrade = Upgrade.A;
			}
			return [new TTCard {
				card = uraniumRound,
			}];
		}
	
    }

    public class UraniumTable : Artifact {
		private static Card? uraniumRound;

		public override void OnReceiveArtifact(State state) {
			AddUranium(state, 3);
		}

		public override void OnCombatStart(State state, Combat combat)
		{
			if (combat.otherShip.ai is Shopkeep) {
				combat.Queue([
					new CardAction {
						artifactPulse = new UraniumIdea().Key()
					},
					new CardAction {
						artifactPulse = new UraniumHonor().Key()
					},
					new CardAction {
						artifactPulse = new UraniumTable().Key()
					},

					new ALoseArtifact {
						artifactType = new UraniumIdea().Key(),
						timer = 0
					},
					new ALoseArtifact {
						artifactType = new UraniumHonor().Key(),
						timer = 0
					},
					new ALoseArtifact {
						artifactType = new UraniumTable().Key(),
						timer = 0
					},
					new AAddArtifact {
						artifact = new UraniumRoundTable(),
						timer = 0,
						artifactPulse = new UraniumRoundTable().Key()
					},
					new AEnergy {
						changeAmount = 1,
						timer = 0
					},
					new ADrawCard {
						count = 1
					}
				]);
			}
		}

		public override List<Tooltip>? GetExtraTooltips()
		{
			if (uraniumRound == null) {
				uraniumRound = (Card)Activator.CreateInstance(BirthdaySecretController.uraniumRound!.Configuration.CardType)!;
				uraniumRound.upgrade = Upgrade.A;
			}
			return [new TTCard {
				card = uraniumRound,
			}];
		}
    }

    public class UraniumRoundTable : Artifact {
		private static Card? uraniumRound;

		public override void OnReceiveArtifact(State state)
		{
			int amount = 20;
			state.ship.hullMax += amount;
			state.ship.baseDraw += 1;
			state.ship.baseEnergy += 1;
			foreach (Artifact item in state.EnumerateAllArtifacts()) {
				amount += item.ModifyHealAmount(amount, state, targetPlayer: true);
			}
			state.ship.Heal(amount);
		}

		public override void OnRemoveArtifact(State state)
		{
			state.ship.baseDraw -= 1;
			state.ship.baseEnergy -= 1;
		}

		public override void OnPlayerPlayCard(int energyCost, Deck deck, Card card, State state, Combat combat, int handPosition, int handCount)
		{
			if (card.GetType() == BirthdaySecretController.uraniumRound!.Configuration.CardType) {
				combat.Queue(new ADrawCard {
					count = 1,
					artifactPulse = Key()
				});
			}
			else if (card is Parry && combat.otherShip.ai is Shopkeep) {
				combat.QueueImmediate(new AStatus {
					status = Status.autododgeRight,
					statusAmount = 1,
					targetPlayer = false
				});
			}
		}

		public override List<Tooltip>? GetExtraTooltips()
		{
			if (uraniumRound == null) {
				uraniumRound = (Card)Activator.CreateInstance(BirthdaySecretController.uraniumRound!.Configuration.CardType)!;
				uraniumRound.upgrade = Upgrade.A;
			}
			return [new TTCard {
				card = uraniumRound,
			}, new TTGlossary("cardtrait.exhaust")];
		}
    }
}