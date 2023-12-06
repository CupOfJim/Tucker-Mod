using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CobaltCoreModding.Definitions;
using CobaltCoreModding.Definitions.ExternalItems;
using CobaltCoreModding.Definitions.ModContactPoints;
using CobaltCoreModding.Definitions.ModManifests;
using HarmonyLib;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework.Graphics;
using TuckerTheSaboteur.cards;

namespace TuckerTheSaboteur
{
    public class MainManifest : IModManifest, ISpriteManifest, ICardManifest, ICharacterManifest, IDeckManifest, IAnimationManifest, IGlossaryManifest, IStatusManifest, IArtifactManifest
    {
        public static MainManifest Instance;

        public IEnumerable<DependencyEntry> Dependencies => new DependencyEntry[0];

        public DirectoryInfo? GameRootFolder { get; set; }
        public Microsoft.Extensions.Logging.ILogger? Logger { get; set; }
        public DirectoryInfo? ModRootFolder { get; set; }

        public string Name => "SoggoruWaffle.Tucker";

        public static Dictionary<string, ExternalSprite> sprites = new Dictionary<string, ExternalSprite>();
        public static Dictionary<string, ExternalAnimation> animations = new Dictionary<string, ExternalAnimation>();
        public static Dictionary<string, ExternalCard> cards = new Dictionary<string, ExternalCard>();
        public static Dictionary<string, ExternalStatus> statuses = new Dictionary<string, ExternalStatus>();
        public static Dictionary<string, ExternalGlossary> glossary = new Dictionary<string, ExternalGlossary>();
        public static ExternalCharacter character;
        public static ExternalDeck deck;

        public void BootMod(IModLoaderContact contact)
        {
            Instance = this;
            var harmony = new Harmony(this.Name);
            harmony.PatchAll();
        }

        public void LoadManifest(ISpriteRegistry artRegistry)
        {
            var filenames = new string[] {
                "character/tucker_neutral_1",

                "icons/Blunt_Attack",
                "icons/Shield_Steal",

                "cards/Psych_Out",
                "cards/Mutual_Gain",
                "cards/Direct_Hit",
            };

            foreach (var filename in filenames) {
                var filepath = Path.Combine(ModRootFolder?.FullName ?? "", "sprites", Path.Combine(filename.Split('/'))+".png");
                var sprite = new ExternalSprite("SoggoruWaffle.Tucker.sprites."+filename, new FileInfo(filepath));
                sprites[filename] = sprite;

                if (!artRegistry.RegisterArt(sprite)) throw new Exception("Error registering sprite " + filename);
            }
        }

        public void LoadManifest(ICardRegistry registry)
        {
            // GOAL:
            // 21 cards
            // 9 common, 7 uncommon, 5 rare
            var cardDefinitions = new ExternalCard[]
            {
                new ExternalCard("SoggoruWaffle.Tucker.cards.Mutual Gain", typeof(MutualGain), sprites["cards/Mutual_Gain"], deck),
            };
            
            foreach(var card in cardDefinitions)
            {
                var name = card.GlobalName.Split('.').LastOrDefault() ?? "FAILED TO FIND NAME";
                card.AddLocalisation(name);
                registry.RegisterCard(card);
                cards[name] = card;
            }
        }

        public void LoadManifest(IDeckRegistry registry)
        {
            var philipColor = 0;
            unchecked { philipColor = (int)0xffe9bd5c; } // TODO: set the hex color you want for Tucker

            deck = new ExternalDeck(
                "SoggoruWaffle.Tucker.TuckerDeck",
                System.Drawing.Color.FromArgb(philipColor),
                System.Drawing.Color.Black,
                sprites["card/tucker_default"],
                sprites["frame_tucker"],
                null
            );
            if (!registry.RegisterDeck(deck)) throw new Exception("Tucker's lost his deck! Cannot proceed, he needs help finding it.");
        }

        public void LoadManifest(ICharacterRegistry registry)
        {
            //var realStartingCards = new Type[] { typeof(OverdriveMod), typeof(RecycleParts) };
            // TODO: initialize realStartingCards like above
            var realStartingCards = new Type[0];
            var allCards = cards.Values.Select(card => card.CardType).ToList();

            character = new ExternalCharacter(
                "SoggoruWaffle.Tucker",
                deck,
                sprites["char_frame_tucker"],
                realStartingCards,
                new Type[0],
                animations["neutral"],
                animations["mini"]
            );

            character.AddNameLocalisation("Tucker");
            // TODO: set color here too, and also write the description
            character.AddDescLocalisation("<c=aaaa00>TUCKER</c>\nHe's <c=aa00bb>cool</c>.");

            if (!registry.RegisterCharacter(character)) throw new Exception("Tucker is lost! Could not register Tucker!");
        }

        public void LoadManifest(IAnimationRegistry registry)
        {
            var animationInfo = new Dictionary<string, IEnumerable<ExternalSprite>>();
            // these are the required animations
            //animationInfo["neutral"] = new ExternalSprite[] { sprites["philip_neutral_0"], sprites["philip_neutral_1"], sprites["philip_neutral_0"], sprites["philip_neutral_1"] };
            //animationInfo["squint"] = new ExternalSprite[] { sprites["philip_squint_0"], sprites["philip_squint_1"], sprites["philip_squint_0"], sprites["philip_squint_1"] };
            //animationInfo["gameover"] = new ExternalSprite[] { sprites["philip_surprise_0"], sprites["philip_surprise_1"], sprites["philip_surprise_0"], sprites["philip_surprise_1"] };
            //animationInfo["mini"] = new ExternalSprite[] { sprites["philip_mini"] };

            foreach (var kvp in animationInfo)
            {
                var animation = new ExternalAnimation(
                    "SoggoruWaffle.Tucker.animations."+kvp.Key,
                    deck,
                    kvp.Key,
                    false,
                    kvp.Value
                );
                animations[kvp.Key] = animation;

                if (!registry.RegisterAnimation(animation)) throw new Exception("Error registering animation " + kvp.Key);
            }
        }

        public void LoadManifest(IGlossaryRegisty registry)
        {
            //RegisterGlossaryEntry(registry, "AReplay", sprites["icon_play_twice"],
            //    "play twice",
            //    "Play all actions prior to the Play Twice action twice."
            //);
        }
        private void RegisterGlossaryEntry(IGlossaryRegisty registry, string itemName, ExternalSprite sprite, string displayName, string description)
        {
            var entry = new ExternalGlossary("SoggoruWaffle.Tucker.Glossary", itemName, false, ExternalGlossary.GlossayType.action, sprite);
            entry.AddLocalisation("en", displayName, description);
            registry.RegisterGlossary(entry);
            glossary[entry.ItemName] = entry;
        }

        public void LoadManifest(IStatusRegistry statusRegistry)
        {
            var fuelLeak = new ExternalStatus("SoggoruWaffle.Tucker.statuses.fuel_leak", true, System.Drawing.Color.Red, null, sprites["icons/Fuel_Leak"], false);
            statusRegistry.RegisterStatus(fuelLeak);
            fuelLeak.AddLocalisation("fuel_leak", "Deals {0} hull damage every time the ship moves any distance.");
            statuses["fuel_leak"] = fuelLeak;

            var buffer = new ExternalStatus("SoggoruWaffle.Tucker.statuses.buffer", true, System.Drawing.Color.Crimson, null, sprites["icons/Buffer"], false);
            statusRegistry.RegisterStatus(buffer);
            buffer.AddLocalisation("buffer", "Grants {0} temp shield and reduces by 1 at the end of every turn.");
            statuses["buffer"] = buffer;
        }

        public void LoadManifest(IArtifactRegistry registry)
        {
            //var wireClippers = new ExternalArtifact("clay.PhilipTheMechanic.Artifacts.WireClippers", typeof(WireClippers), sprites["artifact_wire_clippers"], ownerDeck: deck);
            //wireClippers.AddLocalisation("WIRE CLIPPERS", "All unplayable cards become playable");
            //registry.RegisterArtifact(wireClippers);
        }
    }
}
