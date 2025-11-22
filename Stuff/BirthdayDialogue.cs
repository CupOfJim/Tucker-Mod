using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Newtonsoft.Json;
using TuckerTheSaboteur.actions;

namespace TuckerTheSaboteur;

[HarmonyPatch]
public static partial class BirthdaySecretController
{
	private static bool hasInited = false;

	[HarmonyPostfix]
	[HarmonyPatch(typeof(DB), nameof(DB.SetLocale))]
    private static void DB_SetLocale_Postfix() {
		if (!IsVladPlaying()) return;

        RegisterCharacterNames(DB.currentLocale.strings);
        RegisterDialogue(DB.currentLocale.strings);
    }

	[HarmonyPostfix]
	[HarmonyPatch(typeof(G), nameof(G.DrawLoadScreen))]
	internal static void DrawLoadingScreen_Postfix(MG __instance, ref int __state)
	{
		if (__state <= 0)
			return;
		if ((__instance.loadingQueue?.Count ?? 0) > 0)
			return;
		
		
	}

	
    private static void RegisterChoiceFuncs() {
		{
			MethodInfo info = typeof(BirthdaySecretController).GetMethod("DoYouUnderstand")!;
			DB.eventChoiceFns.Add(info.Name, info);
		}
		{
			MethodInfo info = typeof(BirthdaySecretController).GetMethod("AttackCleo")!;
			DB.eventChoiceFns.Add(info.Name, info);
		}
		{
			MethodInfo info = typeof(BirthdaySecretController).GetMethod("BirthdayVictoryChoice1")!;
			DB.eventChoiceFns.Add(info.Name, info);
		}
		{
			MethodInfo info = typeof(BirthdaySecretController).GetMethod("BirthdayVictoryChoice2")!;
			DB.eventChoiceFns.Add(info.Name, info);
		}
		{
			MethodInfo info = typeof(BirthdaySecretController).GetMethod("EndBirthdayRun")!;
			DB.eventChoiceFns.Add(info.Name, info);
		}
    }

    private static void RegisterCharacterNames(Dictionary<string, string> locale) {
        
        locale["char.Nothing"] = "";
        locale["char.GangSecret"] = "?";
		locale["char.Trio"] = "";
		locale["char.Vlad"] = "Vlad";
    }

    private static void RegisterCharacterAnimations() {
        foreach(IDirectoryInfo directory in Main.Instance.Package.PackageRoot.GetRelative("sprites/stuff").AsDirectory!.Directories) {
            string key = directory.Name;
            if (!DB.charAnimations.TryGetValue(key, out Dictionary<string, List<Spr>>? value)) {
				value = [];
				DB.charAnimations[key] = value;
            }
            foreach(IDirectoryInfo looptag in directory.Directories) {
                List<Spr> sprites = RegisterTalkSprites(directory.Name, looptag.Name);
				value.Add(looptag.Name.ToLower(), sprites);
            }
        }
    }

    private static List<Spr> RegisterTalkSprites(string charName, string looptag)
    {
        var dir = Main.Instance.Package.PackageRoot.GetRelative($"sprites/stuff/{charName}/{looptag}").AsDirectory;
		if (dir != null) {
            return Enumerable.Range(1, 10)
				.Select(i => dir.GetRelativeFile($"{charName}-{looptag}{i}.png"))
				.TakeWhile(f => f.Exists)
				.Select(f => Main.Instance.Helper.Content.Sprites.RegisterSprite(f).Sprite)
				.ToList();
		}
        return [];
    }

	// [HarmonyPostfix]
	// [HarmonyPatch(typeof(StoryNode), nameof(StoryNode.Filter))]
	// private static void StoryNode_Filter_Postfix(bool __result, string key, StoryNode n, State s, StorySearch ctx) {
	// 	if (ctx.lookup?.Contains("shopBefore") == true) {
	// 		Main.Instance.Logger.LogInformation(key);
	// 		Main.Instance.Logger.LogInformation(n.doesNotHaveArtifacts + " " + n.never);
	// 		Main.Instance.Logger.LogInformation("verdict: " + __result);
	// 	}
	// }

    private static void RegisterDialogue(Dictionary<string, string> locale) {
		if (philipDeck == null || eddieDeck == null) return;

		foreach ((string key, StoryNode node) in DB.story.all) {
			if (node.lookup == null) continue;
			if (node.lookup.Contains("shopBefore") && node.hasArtifacts?.Contains(new UraniumTable().Key()) != true) {
				node.doesNotHaveArtifacts ??= [];
				node.doesNotHaveArtifacts.Add(new UraniumTable().Key());
			}
			if (node.lookup.Contains("after_nerd") && node.hasArtifacts?.Contains(new UraniumRoundTable().Key()) != true) {
				node.doesNotHaveArtifacts ??= [];
				node.doesNotHaveArtifacts.Add(new UraniumRoundTable().Key());
			}
		}
		DB.story.all["DefeatedShopkeep"].doesNotHaveArtifacts ??= [];
		DB.story.all["DefeatedShopkeep"].doesNotHaveArtifacts!.Add(new UraniumRoundTable().Key());
		DB.story.all["CrystallizedFriendEvent"].doesNotHaveArtifacts ??= [];
		DB.story.all["CrystallizedFriendEvent"].doesNotHaveArtifacts!.Add(new UraniumIdea().Key());

		Dictionary<string, StoryNode> nodes = [];
		nodes.Add("Birthday_UraniumAfterBrimford", new() {
			type = NodeType.@event,
			oncePerRun = true,
			priority = true,
			zones = [
				"zone_first"
			],
			hasArtifacts = [
				new UraniumIdea().Key()
			],
			lookup = [
				"after_any"
			],
			lines = [
				new CustomSay {
					who = "walrus",
					Text = "Say, before you leave, would y'care to hear about some rumors?",
					flipped = true
				},
				new CustomSay {
					who = eddieDeck.Value.Key(),
					Text = "Eh, I don't do rumors. They're bad vibes.",
				},
				new CustomSay {
					who = "walrus",
					Text = "Oh? Well, I think you'd find them quite... interesting...",
					flipped = true
				},
				new CustomSay {
					who = eddieDeck.Value.Key(),
					Text = "Ehhhh, no, sorry, I left the, uhhh, oven on.",
					loopTag = "annoyed"
				},
				new CustomSay {
					who = "walrus",
					Text = "Welp... Bye then.",
					flipped = true
				},
			]
		});
		nodes.Add("Birthday_GangMessage1", new() {
			type = NodeType.@event,
			oncePerRun = true,
			priority = true,
			lookup = [
				"zone_lawless"
			],
			hasArtifacts = [
				new UraniumIdea().Key()
			],
			lines = [
				new CustomSay {
					who = philipDeck.Value.Key(),
					Text = "Huh, I think my uranium is glowing more than usual!"
				},
				new CustomSay {
					who = "GangSecret",
					Text = "Help him...\nPlease help him...",
					flipped = true
				},
				new CustomSay {
					who = Deck.peri.Key(),
					Text = "Who's there? Show yourself!",
					loopTag = "panic"
				},
				new CustomSay {
					who = "GangSecret",
					Text = "He is a prisoner somewhere, but when I asked, I was told \"None of your business\".",
					flipped = true
				},
				new CustomSay {
					who = "GangSecret",
					Text = "His name is Vlad...",
					flipped = true
				},
				new CustomSay {
					who = eddieDeck.Value.Key(),
					Text = "Vlad? You mean, like, Dracula?",
					loopTag = "squint"
				},
				new CustomSay {
					who = "GangSecret",
					Text = "The shopkeep, Cleo, has done something terrible to the leaderboard seeker... Now only you remain...",
					flipped = true
				},
				new CustomSay {
					who = "GangSecret",
					Text = "Cleo has seized control of the Cobalt and even now is trying to open the time loop.",
					flipped = true
				},
				new CustomSay {
					who = "GangSecret",
					Text = "Vlad is in the dungeon of the shipyard.\nPlease help him.",
					flipped = true
				},
				new CustomSay {
					who = philipDeck.Value.Key(),
					Text = "Cleo? But she seemed so nice.",
					loopTag = "surprise"
				},
				new CustomSay {
					who = "GangSecret",
					Text = "The murine knight holds the piece...",
					flipped = true
				},
				new CustomSay {
					who = "Nothing",
					Text = "... ... ...",
					flipped = true
				},
				new CustomSay {
					who = Deck.peri.Key(),
					Text = "...",
					loopTag = "mad"
				},
				new CustomSay {
					who = Deck.peri.Key(),
					Text = "We're gonna need something higher caliber.",
					loopTag = "mad"
				},
			]
		});
		nodes.Add("Birthday_Ratzo", new() {
			type = NodeType.@event,
			priority = true,
			lookup = [
				"before_knight"
			],
			hasArtifacts = [
				new UraniumIdea().Key()
			],
			lines = [
				new CustomSay {
					who = "knight",
					Text = "Nay, do mine eyes deceive me?",
					flipped = true
				},
				new CustomSay {
					who = "knight",
					Text = "You seek to assemble the <c=artifact>Uranium Round Table</c>?",
					flipped = true
				},
				new CustomSay {
					who = philipDeck.Value.Key(),
					Text = "The what?",
					loopTag = "surprise"
				},
				new CustomSay {
					who = eddieDeck.Value.Key(),
					Text = "Huh?",
					loopTag = "squint"
				},
				new CustomSay {
					who = Deck.peri.Key(),
					Text = "You know about this?"
				},
				new CustomSay {
					who = "knight",
					Text = "I have held this oath for my entire life... and now it is finally time...",
					flipped = true
				},
				new CustomSay {
					who = "knight",
					Text = "Prove your worth, and you may claim my <c=artifact>Honor</c>!",
					flipped = true
				},
			]
		});
		nodes.Add("Birthday_GangMessage2", new() {
			type = NodeType.@event,
			oncePerRun = true,
			priority = true,
			lookup = [
				"zone_three"
			],
			hasArtifacts = [
				new UraniumIdea().Key(),
				new UraniumHonor().Key()
			],
			lines = [
				new CustomSay {
					who = philipDeck.Value.Key(),
					Text = "Guys, it's glowing again!",
					loopTag = "surprise"
				},
				new Jump {
					key = "Birthday_GangMessage2_1"
				}
			]
		});
		nodes.Add("Birthday_GangMessage2_1", new() {
			type = NodeType.@event,
			never = true,
			lines = [
				new CustomSay {
					who = "GangSecret",
					Text = "Crew, listen carefully.\nAll the lifeforms in the sector have fallen under Cleo's influence.",
					flipped = true
				},
				new CustomSay {
					who = Deck.peri.Key(),
					Text = "She'll have a Parry to answer to.",
					loopTag = "vengeful"
				},
				new CustomSay {
					who = "GangSecret",
					Text = "And I fear even worse has befallen Vlad... That shopkeep is a terrifying fiend!",
					flipped = true
				},
				new CustomSay {
					who = "GangSecret",
					Text = "Her powers are strong... ... ... ...",
					flipped = true
				},
				new CustomSay {
					who = "GangSecret",
					Text = "Do you understand?",
					flipped = true
				},
			],
			choiceFunc = "DoYouUnderstand"
		});
		nodes.Add("Birthday_GangMessage2_2", new() {
			type = NodeType.@event,
			never = true,
			lines = [
				new CustomSay {
					who = "GangSecret",
					Text = "Seek out the Buried Relic. Legends tell of treasures with mystical powers that remain from the Heavy Industrial age...",
					flipped = true
				},
				new CustomSay {
					who = "GangSecret",
					Text = "The <c=artifact>Uranium Table</c>, a mighty artifact forged to thwart those with evil hearts, is one...",
					flipped = true
				},
				new CustomSay {
					who = eddieDeck.Value.Key(),
					Text = "Ooo, that sounds up my alley!",
					loopTag = "excited"
				},
				new CustomSay {
					who = "GangSecret",
					Text = "It is said that even now it rests deep in Void's Cradle.",
					flipped = true
				},
				new CustomSay {
					who = "Nothing",
					Text = "... ... ...",
					flipped = true
				},
				new CustomSay {
					who = philipDeck.Value.Key(),
					Text = "The deeper we go, the more this is actually sounding like a bad idea...",
					loopTag = "squint",
				},
				new CustomSay {
					who = Deck.peri.Key(),
					Text = "Maybe... But I have a good feeling about this."
				},
			]
		});
		nodes.Add("Birthday_GangMessage3", new() {
			type = NodeType.@event,
			oncePerRun = true,
			priority = true,
			lookup = [
				"after_stone"
			],
			zones = [
				"zone_three"
			],
			hasArtifacts = [
				new UraniumTable().Key()
			],
			lines = [
				new CustomSay {
					who = eddieDeck.Value.Key(),
					Text = "Wow...",
					loopTag = "excited"
				},
				new CustomSay {
					who = philipDeck.Value.Key(),
					Text = "It's beautiful...",
					loopTag = "excited"
				},
				new CustomSay {
					who = "GangSecret",
					Text = "Hmmm... So at last you have the three artifacts... Good!",
					flipped = true
				},
				new CustomSay {
					who = "GangSecret",
					Text = "Now head for the shipyard.",
					flipped = true
				},
				new CustomSay {
					who = Deck.peri.Key(),
					Text = "And then what?",
					loopTag = "squint"
				},
				new CustomSay {
					who = "GangSecret",
					Text = "If you are the <c=parttrait>true</c> Heroes, the <c=artifact>Uranium Round Table</c> will choose you to wield it...",
					flipped = true
				},
				new CustomSay {
					who = "Nothing",
					Text = "... ... ...",
					flipped = true
				},
				new CustomSay {
					who = eddieDeck.Value.Key(),
					Text = "We're undoubtedly hero material."
				},
			]
		});
		nodes.Add("Birthday_ShopOfDoom", new() {
			type = NodeType.@event,
			priority = true,
			lookup = [
				"shopBefore"
			],
			bg = "BGShop",
			hasArtifacts = [
				new UraniumTable().Key()
			],
			lines = [
				new CustomSay {
					who = Deck.peri.Key(),
					Text = "The jig is up, Cleo.",
					loopTag = "vengeful"
				},
				new CustomSay {
					who = "nerd",
					Text = "Huh?",
					loopTag = "nervous",
					flipped = true
				},
				new CustomSay {
					who = philipDeck.Value.Key(),
					Text = "We know you've got Vlad locked up...",
					loopTag = "squint"
				},
				new CustomSay {
					who = philipDeck.Value.Key(),
					Text = "And we're bustin' 'em out!",
					loopTag = "maniacal"
				},
				new CustomSay {
					who = "nerd",
					Text = "That's none of your business.",
					flipped = true
				},
				new CustomSay {
					who = Deck.peri.Key(),
					Text = "I guess we feel like meddling.",
				},
				new CustomSay {
					who = "nerd",
					Text = "Listen, I don't know how you found out about this, but you're way out of your depth here.",
					flipped = true
				},
				new CustomSay {
					who = eddieDeck.Value.Key(),
					Text = "Talk big all you want. All we gotta do is set up an infinite or use Parry or something.",
					loopTag = "explains"
				},
				new CustomSay {
					who = "nerd",
					Text = "Ha. Ha, ha, hahahah.",
					flipped = true
				},
				new CustomSay {
					who = "nerd",
					Text = "You wouldn't last a second against me, Parry or not. This ain't my first rodeo, buddy.",
					loopTag = "explains",
					flipped = true
				},
				new CustomSay {
					who = "nerd",
					Text = "Sooo I suggest I fix up your deformed little ship and you be on your merry way.",
					flipped = true
				},
			],
			choiceFunc = "AttackCleo"
		});
		nodes.Add("Birthday_CleoMidFight1", new() {
			type = NodeType.combat,
			priority = true,
			oncePerCombat = true,
			allPresent = [
				"nerd",
			],
			hasArtifacts = [
				new UraniumRoundTable().Key()
			],
			lines = [
				new CustomSay {
					who = "nerd",
					Text = "Huh? What's that thing?",
					loopTag = "nervous"
				},
				new CustomSay {
					who = philipDeck.Value.Key(),
					Text = "I have no idea!"
				},
			]
		});
		nodes.Add("Birthday_CleoMidFight2", new() {
			type = NodeType.combat,
			priority = true,
			oncePerCombat = true,
			allPresent = [
				"nerd",
			],
			minTurnsThisCombat = 2,
			hasArtifacts = [
				new UraniumRoundTable().Key()
			],
			lines = [
				new CustomSay {
					who = "nerd",
					Text = "You wanted a celebration, but instead it's your funeral, pal."
				},
			]
		});
		nodes.Add("Birthday_CleoDodgesParry", new() {
			type = NodeType.combat,
			priority = true,
			oncePerCombat = true,
			allPresent = [
				"nerd"
			],
			lastTurnEnemyStatuses = [
				Status.autododgeRight
			],
			hasArtifacts = [
				new UraniumRoundTable().Key()
			],
			lines = [
				new CustomSay {
					who = "nerd",
					Text = "Not this time.",
					loopTag = "explains"
				},
			]
		});
		nodes.Add("Birthday_CleoGonnaDie", new() {
			type = NodeType.combat,
			priority = true,
			oncePerCombat = true,
			minCardsPlayedThisTurn = 6,
			minDamageDealtToEnemyThisTurn = 20,
			allPresent = [
				"nerd"
			],
			hasArtifacts = [
				new UraniumRoundTable().Key()
			],
			lines = [
				new CustomSay {
					who = "nerd",
					Text = "I can't say I was prepared for THIS.",
					loopTag = "nervous"
				},
			]
		});
		
		nodes.Add("Birthday_Victory1", new() {
			type = NodeType.@event,
			priority = true,
			lookup = [
				"after_nerd"
			],
			bg = "BGShopEmpty",
			hasArtifacts = [
				new UraniumRoundTable().Key()
			],
			lines = [
				new CustomSay {
					who = Deck.peri.Key(),
					Text = "Quick, check the debris for survivors! That much radiation will cause all kinds of problems...",
					loopTag = "panic"
				},
				new CustomSay {
					who = philipDeck.Value.Key(),
					Text = "Hey, now... it did the job, didn't it?",
					loopTag = "sheepish",
					flipped = true
				},
				new CustomSay {
					who = eddieDeck.Value.Key(),
					Text = "Wait, something's moving...",
					loopTag = "worried",
					flipped = true
				},
				new CustomSay {
					who = "Vlad",
					Text = "Oh heck, thanks for rescuing me! I've been stuck here for approximately 3.5 hours...",
					loopTag = "scared"
				},
				new CustomSay {
					who = "Vlad",
					Text = "So...",
				},
			],
			choiceFunc = "BirthdayVictoryChoice1"
		});
		nodes.Add("Birthday_Victory1_1", new() {
			type = NodeType.@event,
			bg = "BGShopEmpty",
			lines = [
				new CustomSay {
					who = "Vlad",
					Text = "Did y'all start with Scaffolds, or did you run into Selene?",
				},
				new CustomSay {
					who = eddieDeck.Value.Key(),
					Text = "The Tiderunner came prefitted with additional scaffolding, already wired and ready to go!",
					loopTag = "resting",
					flipped = true
				},
				new CustomSay {
					who = eddieDeck.Value.Key(),
					Text = "I'm glad, because I didn't have to fix the wiring between the ship halves this time.",
					loopTag = "explains",
					flipped = true
				},
				new Jump {
					key = "Birthday_Victory2"
				}
			]
		});
		nodes.Add("Birthday_Victory1_2", new() {
			type = NodeType.@event,
			bg = "BGShopEmpty",
			lines = [
				new CustomSay {
					who = "Vlad",
					Text = "She really got SO fed up with the Parry strat that she kidnapped me?",
				},
				new CustomSay {
					who = Deck.peri.Key(),
					Text = "AND altered the time loop itself to counter it!",
					loopTag = "mad",
					flipped = true,
				},
				new CustomSay {
					who = Deck.peri.Key(),
					Text = "...but she's right:  it became too predictable. Another path led to victory today.",
					loopTag = "nap",
					flipped = true,
				},
				new Jump {
					key = "Birthday_Victory2"
				}
			]
		});
		nodes.Add("Birthday_Victory1_3", new() {
			type = NodeType.@event,
			bg = "BGShopEmpty",
			lines = [
				new CustomSay {
					who = "Vlad",
					Text = "Poor Cleo couldn't stop ALL the contraband!"
				},
				new CustomSay {
					who = philipDeck.Value.Key(),
					Text = "And now she never will, heh heh...",
					loopTag = "maniacal",
					flipped = true,
				},
				new CustomSay {
					who = philipDeck.Value.Key(),
					Text = "She should know: if you get rid of black market stuff, more will take its place.",
					loopTag = "excited",
					flipped = true,
				},
				new Jump {
					key = "Birthday_Victory2"
				}
			]
		});
		nodes.Add("Birthday_Victory2", new() {
			type = NodeType.@event,
			bg = "BGShopEmpty",
			lines = [
				new CustomSay {
					who = "comp",
					Text = "Hey, didn't the weird voice from Philip's uranium say that Cleo took control of the Cobalt? Cleo's gone now...",
					loopTag = "squint"
				},
				new CustomSay {
					who = "Trio",
					Text = "<c=20bdb2>Excuse me? 'Weird'?\n\nYOU'RE the ones who summoned US, thanks...",
					loopTag = "angry-ash",
					flipped = true
				},
				new CustomSay {
					who = "Trio",
					Text = "<c=f6b4f4>Yeah! What's u--",
					loopTag = "angry-dunkyl",
					flipped = true
				},
				new CustomSay {
					who = "Trio",
					Text = "<c=af7a44>NOPE, nice try,\nI'm gonna put an end\nto that pun right now.",
					loopTag = "angry-tormod",
					flipped = true
				},
				new CustomSay {
					who = "Trio",
					Text = "<c=f6b4f4>Rude!",
					loopTag = "angry-dunkyl",
					flipped = true
				},
				new CustomSay {
					who = "Vlad",
					Text = "Wow! Three animals in one character portrait!",
					loopTag = "excited"
				}
			],
			choiceFunc = "BirthdayVictoryChoice2"
		});

		nodes.Add("Birthday_Victory2_1", new() {
			type = NodeType.@event,
			bg = "BGShopEmpty",
			lines = [
				new CustomSay {
					who = "Vlad",
					Text = "Like, why are you IN my run, where I'm trying to get points and stuff. Y'know?",
				},
				new CustomSay {
					who = "Trio",
					Text = "<c=20bdb2>Well, you see...",
					loopTag = "neutral-ash",
					flipped = true
				},
				new CustomSay {
					who = "Trio",
					Text = "<c=f6b4f4>I 3D printed the Uranium\nRound As and Parry\nmonths ago!",
					loopTag = "neutral-dunkyl",
					flipped = true
				},
				new CustomSay {
					who = "Trio",
					Text = "<c=f6b4f4>I wanted to send them then,\nbut Tormod\n\nREALLY\n\nlikes scope creep...",
					loopTag = "neutral-dunkyl",
					flipped = true
				},
				new CustomSay {
					who = "Trio",
					Text = "<c=af7a44>...",
					loopTag = "angry-justtormod",
					flipped = true
				},
				new CustomSay {
					who = "Trio",
					Text = "<c=af7a44>I asked if we could turn this\ninto an ARG of sorts\nfor your...",
					loopTag = "neutral-tormod",
					flipped = true
				},
				new Jump {
					key = "Birthday_Victory3"
				}
			]
		});
		nodes.Add("Birthday_Victory2_2", new() {
			type = NodeType.@event,
			bg = "BGShopEmpty",
			lines = [
				new CustomSay {
					who = "Vlad",
					Text = "Like, it's really neat and all, but... this CAN'T be the daily everyone's playing tonight, right?",
					loopTag = "scared"
				},
				new CustomSay {
					who = "Trio",
					Text = "<c=20bdb2>You assembled the 'Uranium\nIdea' from a rumor;\nthe 'Uranium Honor' from Sir Ratzo;\nand the 'Uranium Table' from Buried Relic!",
					loopTag = "neutral-ash",
					flipped = true
				},
				new CustomSay {
					who = "Trio",
					Text = "<c=20bdb2>When Peri, Philip, and Eddie\nfaced incredible odds to\nrescue you, the hidden power of those artifacts sparked and formed the 'Uranium Round Table', summoning us from afar, and giving the crew the last little bit of help to finish the job.",
					loopTag = "neutral-ash",
					flipped = true
				},
				new CustomSay {
					who = philipDeck.Value.Key(),
					Text = "I always knew the uranium would do right by me!",
					loopTag = "classy"
				},
				new CustomSay {
					who = eddieDeck.Value.Key(),
					Text = "And that, as they say, is that.",
					loopTag = "resting"
				},
				new CustomSay {
					who = eddieDeck.Value.Key(),
					Text = "Right? We're done, right?"
				},
				new CustomSay {
					who = Deck.peri.Key(),
					Text = "...no, not yet. Vlad, the loop's not over until we celebrate your...",
					loopTag = "nap"
				},
				new Jump {
					key = "Birthday_Victory3"
				}
			]
		});
		nodes.Add("Birthday_Victory2_3", new() {
			type = NodeType.@event,
			bg = "BGShopEmpty",
			lines = [
				new CustomSay {
					who = "Vlad",
					Text = "Like, isn't it just looming in the distance... waiting and watching?",
					loopTag = "scared"
				},
				new CustomSay {
					who = Deck.peri.Key(),
					Text = "If Dizzy were here, he'd probably go on about how the Cobalt is a conscious entity, and all his research, and so on...",
					loopTag = "nap"
				},
				new CustomSay {
					who = "comp",
					Text = "You have NO IDEA.",
					loopTag = "squint"
				},
				new CustomSay {
					who = "comp",
					Text = "Fortunately, that's not a concern right now! Simply... do not worry about it!",
				},
				new CustomSay {
					who = "Trio",
					Text = "<c=ffffff>...",
					loopTag = "neutral-all",
					flipped = true
				},
				new CustomSay {
					who = eddieDeck.Value.Key(),
					Text = "...",
					loopTag = "resting"
				},
				new CustomSay {
					who = philipDeck.Value.Key(),
					Text = "Yeah, Vlad, it'll be fine. The loop's gonna be over once we celebrate your...",
					loopTag = "classy"
				},
				new Jump {
					key = "Birthday_Victory3"
				}
			]
		});
		nodes.Add("Birthday_Victory3", new() {
			type = NodeType.@event,
			bg = "BGShopEmpty",
			lines = [
				new CustomSay {
					who = "Trio",
					Text = "<c=ffffff>...BIRTHDAY!",
					loopTag = "cake-all",
					flipped = true
				},
				new CustomSay {
					who = "Vlad",
					Text = "Oh no...",
					loopTag = "scared"
				},
				new CustomSay {
					who = "Vlad",
					Text = "OH YES! I GET CAKE, TOO!",
					loopTag = "cake"
				},
				new CustomSay {
					who = "Trio",
					Text = "<c=af7a44>...Actually, you might\nwant to take off your\nheadset for this part. We ALL get cake here at The Den!",
					loopTag = "cake-tormod",
					flipped = true
				},
				new CustomTitleCard {
					Text = "T+ one delicious cake"
				},
				new Wait {
					secs = 3
				},
				new CustomSay {
					who = "Trio",
					Text = "<c=af7a44>From Ash, Dunkyl, Shockah,\nArin, Jaz, Azure, and myself:\nWe hope you have a wonderful birthday and a healthy and happy year!",
					loopTag = "neutral-tormod",
					flipped = true
				},
				new CustomSay {
					who = "Trio",
					Text = "<c=af7a44>We all hope you enjoyed this\nas much as we loved putting\nit together for you!",
					loopTag = "neutral-tormod",
					flipped = true
				},
				new CustomSay {
					who = "Trio",
					Text = "<c=ffffff>Happy birthday, Vlad!",
					loopTag = "cake-all",
					flipped = true
				},
			],
			choiceFunc = "EndBirthdayRun"
		});

		foreach ((string key, StoryNode node) in nodes) {
			if (!hasInited)
				DB.story.all.Add(key, node);
			int i = 0;
			foreach (Instruction instr in node.lines) {
				if (instr is CustomSay cs) {
					cs.hash = i.ToString();
					locale[$"{key}:{i++}"] = cs.Text;
				}
			}
		}
    }

	public static List<Choice> DoYouUnderstand(State s) {
		return [
			new Choice
			{
				label = Main.Instance.Localizations.Localize(["choiceFns", "doYouUnderstand", "yes"]),
				key = "Birthday_GangMessage2_2"
			},
			new Choice
			{
				label = Main.Instance.Localizations.Localize(["choiceFns", "doYouUnderstand", "no"]),
				key = "Birthday_GangMessage2_1"
			}
		];
	}

	public static List<Choice> AttackCleo(State s) {
		return [
			new Choice
			{
				label = Main.Instance.Localizations.Localize(["choiceFns", "attackCleo"]),
				key = "Birthday_AttackCleo",
				actions = [
					new AStartCombat {
						ai = new Shopkeep()
					}
				]
			}
		];
	}

	public static List<Choice> BirthdayVictoryChoice1(State s) => new List<int>([1, 2, 3]).Select(i =>
		new Choice {
			label = Main.Instance.Localizations.Localize(["choiceFns", "birthdayVictoryChoice1", i.ToString()]),
			key = "Birthday_Victory1_" + i
		}
	).ToList();

	public static List<Choice> BirthdayVictoryChoice2(State s) => new List<int>([1, 2, 3]).Select(i =>
		new Choice {
			label = Main.Instance.Localizations.Localize(["choiceFns", "birthdayVictoryChoice2", i.ToString()]),
			key = "Birthday_Victory2_" + i
		}
	).ToList();

	public static List<Choice> EndBirthdayRun(State s) {
		return [
			new Choice
			{
				label = Main.Instance.Localizations.Localize(["choiceFns", "endBirthdayRun"]),
				actions = [
					new AWin()
				]
			}
		];
	}
}