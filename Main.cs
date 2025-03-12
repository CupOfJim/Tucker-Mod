using HarmonyLib;
using Nickel;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using TheJazMaster.MoreDifficulties;
using TuckerTheSaboteur.cards;
using TuckerTheSaboteur.Artifacts;
using Nanoray.PluginManager;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Globalization;
using TuckerTheSaboteur.actions;

namespace TuckerTheSaboteur;

public class Main : SimpleMod
{
    public IMoreDifficultiesApi? MoreDifficultiesApi { get; private set; }

    public static Main Instance { get; private set; } = null!;

    internal Harmony Harmony = null!;

    internal ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations { get; }
    internal ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations { get; }


    internal IPlayableCharacterEntryV2 TuckerCharacter { get; }
    internal IDeckEntry TuckerDeck { get; }

    internal Spr TuckerPortrait;
    internal Spr TuckerPortraitMini;
    internal Spr TuckerFrame;
    internal Spr TuckerCardBorder;

    internal Spr BluntAttackSprite;
    internal Spr BluntAttackFailSprite;
    internal Spr ShieldStealSprite;
    internal Spr OffsetShotLeftSprite;
    internal Spr OffsetShotRightSprite;

    internal IStatusEntry FuelLeakStatus;
    internal IStatusEntry BufferStatus;

    internal List<Type> CardTypes = [
        typeof(Curveball), typeof(TractorBeam), typeof(DirectHit), typeof(Hook), typeof(Lockon), typeof(MutualGain), typeof(PressureGun), typeof(QuantumCannon), typeof(Sabotage),
        typeof(CounterfeitSwap), typeof(Juggle), typeof(LieLow), typeof(Lockpick), typeof(NavOverride), typeof(RubberBullet), typeof(SprayandPray),
        typeof(FortressMode), typeof(KnowThyEnemy), typeof(PsychOut), typeof(ThreadtheNeedle), typeof(Cripple),
        typeof(Counterattack), typeof(Misdirection)
    ];
    internal List<Type> ArtifactTypes = [
        typeof(ArtofWar), typeof(Brick), typeof(CommJammer), typeof(HoloWall),
        typeof(AntiqueMotor)
    ];

    public Main(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
    {
        MoreDifficultiesApi = helper.ModRegistry.GetApi<IMoreDifficultiesApi>("TheJazMaster.MoreDifficulties");

        Instance = this;
        Harmony = new(package.Manifest.UniqueName);
        Harmony.PatchAll();

        AnyLocalizations = new JsonLocalizationProvider(
            tokenExtractor: new SimpleLocalizationTokenExtractor(),
            localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"I18n/{locale}.json").OpenRead()
        );
        Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
            new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(AnyLocalizations)
        );

        TuckerPortrait = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/Character/tucker_neutral_1.png")).Sprite;
        TuckerPortraitMini = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("Sprites/character/mini_tucker.png")).Sprite;
		TuckerFrame = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("sprites/character/tucker_border.png")).Sprite;
        TuckerCardBorder = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("sprites/cards/Card_Border.png")).Sprite;

        ShieldStealSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("sprites/icons/Shield_Steal.png")).Sprite;
        BluntAttackSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("sprites/icons/Blunt_Attack.png")).Sprite;
        BluntAttackFailSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("sprites/icons/Blunt_Attack_Fail.png")).Sprite;
        OffsetShotLeftSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("sprites/icons/Offset_Shot_Left.png")).Sprite;
        OffsetShotRightSprite = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("sprites/icons/Offset_Shot_Right.png")).Sprite;
  
        FuelLeakStatus = helper.Content.Statuses.RegisterStatus("FuelLeak", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("sprites/icons/Fuel_Leak.png")).Sprite,
				color = new("ff0000")
			},
			Name = AnyLocalizations.Bind(["status", "FuelLeak", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "FuelLeak", "description"]).Localize
		});
        BufferStatus = helper.Content.Statuses.RegisterStatus("Buffer", new()
		{
			Definition = new()
			{
				icon = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("sprites/icons/Buffer.png")).Sprite,
				color = new("fff8dc"),
				isGood = true,
				affectedByTimestop = true
			},
			Name = AnyLocalizations.Bind(["status", "Buffer", "name"]).Localize,
			Description = AnyLocalizations.Bind(["status", "Buffer", "description"]).Localize
		});

		TuckerDeck = helper.Content.Decks.RegisterDeck("Tucker", new()
		{
			Definition = new() { color = new Color("e9bd5c"), titleColor = Colors.black },
			DefaultCardArt = StableSpr.cards_colorless,
			BorderSprite = TuckerCardBorder,
			Name = AnyLocalizations.Bind(["character", "name"]).Localize
		});

        foreach (var cardType in CardTypes)
			AccessTools.DeclaredMethod(cardType, nameof(IRegisterableCard.Register))?.Invoke(null, [helper]);
		foreach (var artifactType in ArtifactTypes)
			AccessTools.DeclaredMethod(artifactType, nameof(IRegisterableArtifact.Register))?.Invoke(null, [helper]);

        TuckerCharacter = helper.Content.Characters.V2.RegisterPlayableCharacter("Tucker", new()
		{
			Deck = TuckerDeck.Deck,
			Description = AnyLocalizations.Bind(["character", "description"]).Localize,
			BorderSprite = TuckerFrame,
			Starters = new StarterDeck {
				cards = [ new Sabotage(), new DirectHit() ]
			},
			NeutralAnimation = RegisterAnimation(helper, "Neutral").Configuration,
			MiniAnimation = RegisterAnimation(helper, "Mini").Configuration,
		});
        MoreDifficultiesApi?.RegisterAltStarters(TuckerDeck.Deck, new() { cards = [new QuantumCannon(), new MutualGain()] });

		RegisterAnimation(helper, "Squint");
		RegisterAnimation(helper, "Gameover");
    }

    private ICharacterAnimationEntryV2 RegisterAnimation(IModHelper helper, string name)
    {
        return helper.Content.Characters.V2.RegisterCharacterAnimation(name, new()
		{
			CharacterType = TuckerDeck.Deck.Key(),
			LoopTag = name.ToLower(),
			Frames = Enumerable.Range(1, 100)
				.Select(i => Instance.Package.PackageRoot.GetRelativeFile($"Sprites/character/tucker_{name.ToLower()}_{i}.png"))
				.TakeWhile(f => f.Exists)
				.Select(f => helper.Content.Sprites.RegisterSprite(f).Sprite)
				.ToList()
		});
    }
    
	public override object? GetApi(IModManifest requestingMod)
		=> new ApiImplementation();
}