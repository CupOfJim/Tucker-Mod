using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Nickel;

namespace TuckerTheSaboteur;

[HarmonyPatch]
public static partial class BirthdaySecretController
{
    internal const int day = 20231; 
    internal static Deck? eddieDeck = null;
    internal static Deck? philipDeck = null;

    internal static DailyDescriptor? theDescriptor;

    private static bool IsSpecialDaily(State s) {
        return IsSpecialDaily(s.dailyDay ?? 0);
    }
    private static bool IsSpecialDaily(int dailyDay) {
        return IsVladPlaying() &&  dailyDay == day;
    }

    // Me: 76561198138847183
    // Vlad: 76561198165104664
    private static bool IsVladPlaying() {
        return true;
        // return SteamClient.SteamId == 76561198165104664;
    }

    internal static void Init(IModHelper helper) {
        if (!IsVladPlaying()) return;

        foreach (IModManifest manifest in helper.ModRegistry.LoadedMods.Values) {
            if (manifest.UniqueName == "TheJazMaster.Eddie") {
                eddieDeck = helper.ModRegistry.GetModHelper(manifest).Content.Characters.V2.RegisteredPlayableCharacters.GetValueOrDefault("Eddie.Character.Eddie")!.Configuration.Deck;
            }
            if (manifest.UniqueName == "clay.PhilipTheMechanic") {
                philipDeck = helper.ModRegistry.GetModHelper(manifest).Content.Characters.V2.RegisteredPlayableCharacters.GetValueOrDefault("Philip")!.Configuration.Deck;
                uraniumRound = helper.ModRegistry.GetModHelper(manifest).Content.Cards.RegisteredCards["UraniumRound"];
                blackMarketParts = helper.ModRegistry.GetModHelper(manifest).Content.Cards.RegisteredCards["BlackMarketParts"];
            }
        }
        RegisterArtifacts(helper);

        RegisterCharacterNames(DB.currentLocale.strings);
        RegisterDialogue(DB.currentLocale.strings);

		if (hasInited) return;
		hasInited = true;

		RegisterCharacterAnimations();
        RegisterChoiceFuncs();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Events), nameof(Events.ShopFightBackOut))]
    private static void Events_ShopFightBackOut_Postfix(State s, List<Choice> __result) {
        if (!IsSpecialDaily(s)) return;

        __result.RemoveAt(0);
    }

	[HarmonyPostfix]
	[HarmonyPatch(typeof(Events), nameof(Events.CafeSmuggler))]
	private static void Events_CafeSmuggler_Postfix(State s, List<Choice> __result) {
		if (!IsSpecialDaily(s)) return;

        __result[0].chance = 1;
        if (__result[0].actions.FirstOrDefault() is AArtifactOffering aArtifactOffering)
            aArtifactOffering.canSkip = false;
        var oldLabel = __result[1].label;
        __result[1] = Mutil.DeepCopy(__result[0]);
        __result[1].label = oldLabel;
	}

    [HarmonyPrefix]
    [HarmonyPatch(typeof(DailyDescriptor), nameof(DailyDescriptor.Create))]
    // 21st: 20229
    private static bool DailyDescriptor_Create_Prefix(int daySince1970, ref DailyDescriptor __result) {
        if (!IsSpecialDaily(daySince1970)) return true;

        // CHECK DAY FOR TESTING
        // if (daySince1970 != 20221) return true;
        if (theDescriptor != null) {
            __result = theDescriptor;
            return false;
        }

		Rand rand = new((uint)(daySince1970 * 7919));
		int num = rand.NextInt();

        theDescriptor = new DailyDescriptor {
            shipKey = StarterShip.ships["boat"].ship.key,
            daySince1970 = daySince1970,
			seed = (uint)num,
            crew = [
                eddieDeck ?? Deck.dizzy,
                philipDeck ?? Deck.riggs,
                Deck.peri
            ],
            extraArtifactKeys = [
                "DailyModeTracker", "DailyScaffold", "DailyBossArtifactTreat"
            ],
            difficulty = 3
        };
        __result = theDescriptor;
        return false;
    }

    private static void SwapNodes(Dictionary<Vec, Marker> markers, Vec first, Vec second) {
        (markers[first].paths, markers[second].paths) = (markers[second].paths, markers[first].paths);
        (markers[first], markers[second]) = (markers[second], markers[first]);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MapBase), nameof(MapBase.Populate))]
    private static void MapBase_Populate_Postfix(State s, Rand rng, MapBase __instance) {
        if (!IsSpecialDaily(s)) return;
        
        if (__instance is MapFirst) {
			// Swap event with enemy so it's unavoidable
            SwapNodes(__instance.markers, new Vec(3, 2), new Vec(3, 1));
		}
		else if (__instance is MapLawless) {
			// Swap enemy with event so it's unavoidable
            SwapNodes(__instance.markers, new Vec(4, 4), new Vec(4, 6));
        }
		else if (__instance is MapThree) {
			// Swap elite with repair yard and enemy so they're unavoidable
            SwapNodes(__instance.markers, new Vec(4, 6), new Vec(3, 4));
            SwapNodes(__instance.markers, new Vec(0, 3), new Vec(2, 6));
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MapEvent), nameof(MapEvent.MakeRoute))]
    private static void MapEvent_MakeRoute_Postfix(State s, Vec coord, ref Route __result) {
        if (!IsSpecialDaily(s)) return;

        if (s.map is MapFirst) {
            __result = coord.y switch {
                2 => coord.x switch {
                    3 => Dialogue.MakeDialogueRouteOrSkip(s, "CafeSmuggler", OnDone.map),
                    _ => __result
                },
                6 => coord.x switch {
                    1 => Dialogue.MakeDialogueRouteOrSkip(s, "CafeSmuggler", OnDone.map),
                    _ => __result
                },
                _ => __result
            };
        }
        else if (s.map is MapLawless) {

        }
        else if (s.map is MapThree) {
            
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MapBase), nameof(MapBase.PullEnemyFromHat))]
    private static void MapBase_PullEnemyFromHat_Postfix(State s, BattleType battleType, Vec coord, ref AI __result) {
        if (!IsSpecialDaily(s)) return;

        if (s.map is MapLawless) {
            if (coord == new Vec(0, 4) || coord == new Vec(2, 4) || coord == new Vec(4, 4)) {
                __result = new Knight();
            }
        }
        else if (s.map is MapThree) {
            if (coord == new Vec(2, 6) || coord == new Vec(4, 6)) {
                __result = new StoneGuy();
            }
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MapLawless), nameof(MapLawless.GetEnemyPools))]
    private static void MapLawless_GetEnemyPools_Postfix(State s, MapBase.MapEnemyPool __result) {
        if (!IsSpecialDaily(s)) return;

        __result.normal.RemoveAll(ai => ai is Knight);
    }
    [HarmonyPostfix]
    [HarmonyPatch(typeof(MapThree), nameof(MapThree.GetEnemyPools))]
    private static void MapThree_GetEnemyPools_Postfix(State s, MapBase.MapEnemyPool __result) {
        if (!IsSpecialDaily(s)) return;

        __result.elites.RemoveAll(ai => ai is StoneGuy);
    }

	[HarmonyPostfix]
	[HarmonyPatch(typeof(ArtifactReward), nameof(ArtifactReward.GetOffering))]
	private static void ArtifactReward_GetOffering_Postfix(ref List<Artifact> __result, State s, int count, Deck? limitDeck = null, List<ArtifactPool>? limitPools = null, Rand? rngOverride = null) {
        if (!IsSpecialDaily(s)) return;

		if (s.map is MapFirst && s.map.markers[s.map.currentLocation].contents is MapEvent && s.route is Dialogue d && d.ctx.script == "CafeSmuggler" && !s.EnumerateAllArtifacts().OfType<UraniumIdea>().Any()) {
            __result = [new UraniumIdea()];
            d.onDone = OnDone.visitCurrent;
        }
		if (s.map is MapLawless && s.map.markers[s.map.currentLocation].contents is MapBattle mb && mb.ai is Knight && !s.EnumerateAllArtifacts().OfType<UraniumHonor>().Any()) {
            __result = [new UraniumHonor()];
        }
		if (s.map is MapThree && s.map.markers[s.map.currentLocation].contents is MapBattle mb2 && mb2.ai is StoneGuy && !s.EnumerateAllArtifacts().OfType<UraniumTable>().Any()) {
            __result = [new UraniumTable()];
        }
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(CardReward), nameof(CardReward.GetOffering))]
	private static void CardReward_GetOffering_Postfix(List<Card> __result, State s, int count, Deck? limitDeck = null, BattleType battleType = BattleType.Normal, Rarity? rarityOverride = null, bool? overrideUpgradeChances = null, bool makeAllCardsTemporary = false, bool inCombat = false, int discount = 0, bool isEvent = false) {
        if (!IsSpecialDaily(s)) return;

		if (s.map is MapFirst && s.map.currentLocation.y == 3 && limitDeck == null && rarityOverride == null) {
            __result[0] = (Card)Activator.CreateInstance(blackMarketParts!.Configuration.CardType)!;
            __result[0].drawAnim = 1;
            __result[0].flipAnim = 1;
        }
		if (s.map is MapLawless && s.map.currentLocation.y == 7 && limitDeck == null && rarityOverride == null) {
            __result[0] = new Parry();
            __result[0].drawAnim = 1;
            __result[0].flipAnim = 1;
        }
    }

	[HarmonyPostfix]
	[HarmonyPatch(typeof(Combat), nameof(Combat.PlayerWon))]
    private static void Combat_PlayerWon_Postfix(G g, Combat __instance) {
        if (!IsSpecialDaily(g.state)) return;

        if (__instance.otherShip.ai is Knight && !g.state.EnumerateAllArtifacts().OfType<UraniumHonor>().Any()) {
            g.state.rewardsQueue.Queue(new AArtifactOffering {
                amount = 1,
                canSkip = false
            });
        }
    }




}