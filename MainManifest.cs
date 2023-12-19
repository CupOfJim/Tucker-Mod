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
using TuckertheSabotuer.Artifacts;

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
                "character/tucker_neutral_2",
                "character/tucker_neutral_3",
                "character/tucker_neutral_4",
                "character/tucker_squint_1",
                "character/tucker_squint_2",
                "character/tucker_squint_3",
                "character/tucker_squint_4",
                "character/tucker_death",

                "icons/Blunt_Attack",
                "icons/Blunt_Attack_Fail",
                "icons/Shield_Steal",
                "icons/Incoming",
                "icons/Offset_Shot_Left",
                "icons/Offset_Shot_Right",
                "icons/Replay",
                "icons/Buffer",
                "icons/Fuel_Leak",
                "icons/War_of_War",
                "icons/Holo-Wall",
                "icons/Antique_Motor",
                "icons/Brick",
                "icons/Comm_Jammer",

                "cards/Psych_Out",
                "cards/Mutual_Gain",
                "cards/Direct_Hit",
                "cards/Counter-attack",
                "cards/Counterfeit_Swap",
                "cards/Cripple",
                "cards/Cureball",
                "cards/Fortress_Mode",
                "cards/Hook",
                "cards/Juggle",
                "cards/Know_Thy_Enemy",
                "cards/Lie_Low",
                "cards/Lock-on",
                "cards/Lockpick",
                "cards/Misdirection",
                "cards/Nav_Override",
                "cards/Pressure_Gun",
                "cards/Quantum_Cannon",
                "cards/Sabotage",
                "cards/Spray_and_Pray",
                "cards/Thread_the_Needle",
                "cards/Tractor_Beam",
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
                new ExternalCard("SoggoruWaffle.Tucker.cards.Counter-attack", typeof(Counterattack), sprites["cards/Counter-attack"], deck),
                new ExternalCard("SoggoruWaffle.Tucker.cards.Counterfeit Swap", typeof(CounterfeitSwap), sprites["cards/Counterfeit_Swap"], deck),
                new ExternalCard("SoggoruWaffle.Tucker.cards.Cripple", typeof(Cripple), sprites["cards/Cripple"], deck),
                new ExternalCard("SoggoruWaffle.Tucker.cards.Curveball", typeof(Curveball), sprites["cards/Curveball"], deck),
                new ExternalCard("SoggoruWaffle.Tucker.cards.Direct Hit", typeof(DirectHit), sprites["cards/Direct_Hit"], deck),
                new ExternalCard("SoggoruWaffle.Tucker.cards.Fortress Mode", typeof(FortressMode), sprites["cards/Fortress_Mode"], deck),
                new ExternalCard("SoggoruWaffle.Tucker.cards.Hook", typeof(Hook), sprites["cards/Hook"], deck),
                new ExternalCard("SoggoruWaffle.Tucker.cards.Juggle", typeof(Juggle), sprites["cards/Juggle"], deck),
                new ExternalCard("SoggoruWaffle.Tucker.cards.Lie Low", typeof(LieLow), sprites["cards/Lie_Low"], deck),
                new ExternalCard("SoggoruWaffle.Tucker.cards.Lockpick", typeof(Lockpick), sprites["cards/Lockpick"], deck),
                new ExternalCard("SoggoruWaffle.Tucker.cards.Misdirection", typeof(Misdirection), sprites["cards/Misdirection"], deck),
                new ExternalCard("SoggoruWaffle.Tucker.cards.Nav Override", typeof(NavOverride), sprites["cards/Nav_Override"], deck),
                new ExternalCard("SoggoruWaffle.Tucker.cards.Pressure Gun", typeof(PressureGun), sprites["cards/Pressure_Gun"], deck),
                new ExternalCard("SoggoruWaffle.Tucker.cards.Psych Out", typeof(PsychOut), sprites["cards/Psych_Out"], deck),
                new ExternalCard("SoggoruWaffle.Tucker.cards.Quantum Cannon", typeof(QuantumCannon), sprites["cards/Quantum_Cannon"], deck),
                new ExternalCard("SoggoruWaffle.Tucker.cards.Rubber Bullet", typeof(RubberBullet), sprites["cards/Rubber_Bullet"], deck),
                new ExternalCard("SoggoruWaffle.Tucker.cards.Sabotage", typeof(Sabotage), sprites["cards/Sabotage"], deck),
                new ExternalCard("SoggoruWaffle.Tucker.cards.Spray and Pray", typeof(SprayandPray), sprites["cards/Spray_and_Pray"], deck),
                new ExternalCard("SoggoruWaffle.Tucker.cards.Thread the Needle", typeof(ThreadtheNeedle), sprites["cards/Thread_the_Needle"], deck),
                new ExternalCard("SoggoruWaffle.Tucker.cards.Tractor Beam", typeof(TractorBeam), sprites["cards/Tractor_Beam"], deck),
                new ExternalCard("SoggoruWaffle.Tucker.cards.Lock-on", typeof(Lockon), sprites["cards/Lock-on"], deck),
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
            var tuckerColor = 0;
            unchecked { tuckerColor = (int)0xffe9bd5c; }

            deck = new ExternalDeck(
                "SoggoruWaffle.Tucker.TuckerDeck",
                System.Drawing.Color.FromArgb(tuckerColor),
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
            var realStartingCards = new Type[] { typeof(Sabotage), typeof(DirectHit) };
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
            character.AddDescLocalisation("<c=e9bd5c>TUCKER</c>\nA retired sabotuer. His cards manipulate <c=aa00bb>the enemy's shield</c> and <c=aa00bb>positioning</c>.");

            if (!registry.RegisterCharacter(character)) throw new Exception("Tucker is lost! Could not register Tucker!");
        }

        public void LoadManifest(IAnimationRegistry registry)
        {
            var animationInfo = new Dictionary<string, IEnumerable<ExternalSprite>>();
            // these are the required animations
            animationInfo["neutral"] = new ExternalSprite[] { sprites["tucker_neutral_1"], sprites["tucker_neutral_2"], sprites["tucker_neutral_3"], sprites["tucker_neutral_4"] };
            animationInfo["squint"] = new ExternalSprite[] { sprites["tucker_squint_1"], sprites["tucker_squint_2"], sprites["tucker_squint_3"], sprites["tucker_squint_4"] };
            animationInfo["gameover"] = new ExternalSprite[] { sprites["tucker_death"] };
            animationInfo["mini"] = new ExternalSprite[] { sprites["mini_tucker"] };

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
            RegisterGlossaryEntry(registry, "AReplay", sprites["replay"],
                "play twice",
                "Play all actions prior to the Play Twice action twice."
            );
            RegisterGlossaryEntry(registry, "ABluntAttack", sprites["blunt_attack"],
                "blunt attack",
                "Completely negated by shields. This goes for effects it would otherwise apply, as well."
            );
            RegisterGlossaryEntry(registry, "AShieldSteal", sprites["shield_steal"],
                "shield steal",
                "Steal up to {0} shield from the enemy and apply it to yourself. If they have no shield, steal temp shield instead."
            );
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
            fuelLeak.AddLocalisation("fuel_leak", "Take {0} damage every time the ship moves any distance.");
            statuses["fuel_leak"] = fuelLeak;

            var buffer = new ExternalStatus("SoggoruWaffle.Tucker.statuses.buffer", true, System.Drawing.Color.Crimson, null, sprites["icons/Buffer"], false);
            statusRegistry.RegisterStatus(buffer);
            buffer.AddLocalisation("buffer", "Grants {0} temp shield and reduces by 1 at the end of every turn.");
            statuses["buffer"] = buffer;
        }

        public void LoadManifest(IArtifactRegistry registry)
        {
            var antiqueMotor = new ExternalArtifact("SoggoruWaffle.TuckerTheSabotuer.Artifacts.Antique_Motor", typeof(AntiqueMotor), sprites["Antique_Motor"], ownerDeck: deck);
            antiqueMotor.AddLocalisation("ANTIQUE MOTOR", "Gain 1 extra <c=energy>ENERGY</c> every turn. <c=downside>Gain 1</c> <c=status>FUEL LEAK</c> <c=downside>on the first turn</c>.");
            registry.RegisterArtifact(antiqueMotor);

            var brick = new ExternalArtifact("SoggoruWaffle.TuckerTheSabotuer.Artifacts.Brick", typeof(Brick), sprites["Brick"], ownerDeck: deck);
            brick.AddLocalisation("BRICK", "<c=card>BLUNT ATTACKS</c> deal +1 damage.");
            registry.RegisterArtifact(brick);

            var holoWall = new ExternalArtifact("SoggoruWaffle.TuckerTheSabotuer.Artifacts.Holo-Wall", typeof(HoloWall), sprites["Holo-Wall"], ownerDeck: deck);
            holoWall.AddLocalisation("HOLO-WALL", "Gain 3 <c=status>Buffer</c> on the first turn.");
            registry.RegisterArtifact(holoWall);

            var commJammer = new ExternalArtifact("SoggoruWaffle.TuckerTheSabotuer.Artifacts.Comm_Jammer", typeof(CommJammer), sprites["Comm_Jammer"], ownerDeck: deck);
            commJammer.AddLocalisation("INTERCEPTOR", "Take 1 less damage from <c=drone>midrow objects</c>.");
            registry.RegisterArtifact(commJammer);

            var artofWar = new ExternalArtifact("SoggoruWaffle.TuckerTheSabotuer.Artifacts.Art_of_War", typeof(ArtofWar), sprites["Art_of_War"], ownerDeck: deck);
            artofWar.AddLocalisation("ART OF WAR", "When out of shields, gain a <c=card>Counter-attack</c>.");
            registry.RegisterArtifact(artofWar);
        }
    }
}
