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
using TheJazMaster.Bucket;
using Nickel.Essentials;
using TuckerTheSaboteur;
using TuckerMod.tuckerArtifacts.Duos;

namespace TuckerTheSaboteur;

public class Main : SimpleMod
{
    internal IMoreDifficultiesApi? MoreDifficultiesApi { get; private set; }
    internal IDuoArtifactsApi? DuoArtifactsApi { get; private set; }
    internal IKokoroApi? KokoroApi { get; private set; }
    internal IEssentialsApi? EssentialsApi { get; private set; }
    internal IMarielleApi? MarielleApi { get; private set; }
	internal ITyAndSashaApi? TyAndSashaApi { get; }
	internal IJohnsonApi? JohnsonApi { get; }
    internal IDestinyApi? DestinyApi { get; }
	internal IBucketApi? BucketApi { get; }
	internal IDynaApi? DynaApi { get; }

    public static Main Instance { get; private set; } = null!;

    internal Harmony Harmony = null!;

    internal ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations { get; }
    internal ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations { get; }


    internal IPlayableCharacterEntryV2 TuckerCharacter { get; }
    internal IDeckEntry TuckerDeck { get; }

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
        typeof(AntiqueMotor),

        typeof(AlwaysBeCountering), typeof(Cryostasis), typeof(GrouchoStache), typeof(Hyperlink),
        typeof(NavyTraining), typeof(PetBed), typeof(ProPain), typeof(RCRemote), typeof(ShieldBash),
        typeof(ToolsOfTheTrade), typeof(WarTable), typeof(Whetstone), typeof(WireTap)
    ];

    public Main(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
    {
        MoreDifficultiesApi = helper.ModRegistry.GetApi<IMoreDifficultiesApi>("TheJazMaster.MoreDifficulties");
        DuoArtifactsApi = helper.ModRegistry.GetApi<IDuoArtifactsApi>("Shockah.DuoArtifacts");
        EssentialsApi = helper.ModRegistry.GetApi<IEssentialsApi>("Nickel.Essentials");
        KokoroApi = helper.ModRegistry.GetApi<IKokoroApi>("Shockah.Kokoro");
        MarielleApi = helper.ModRegistry.GetApi<IMarielleApi>("rtf.Marielle");
	    TyAndSashaApi = helper.ModRegistry.GetApi<ITyAndSashaApi>("TheJazMaster.TyAndSasha");
	    JohnsonApi = helper.ModRegistry.GetApi<IJohnsonApi>("Shockah.Johnson");
        DestinyApi = helper.ModRegistry.GetApi<IDestinyApi>("Shockah.Destiny");
	    BucketApi = helper.ModRegistry.GetApi<IBucketApi>("TheJazMaster.Bucket");
	    DynaApi = helper.ModRegistry.GetApi<IDynaApi>("Shockah.Dyna");

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

        Helper.Events.OnModLoadPhaseFinished += OnModLoadPhaseFinished;
    }

    [EventPriority(Priority.Last)]
    private static void OnModLoadPhaseFinished(object? arg, ModLoadPhase phase) {
        if (phase != ModLoadPhase.AfterDbInit) return;
        BirthdaySecretController.Init(Instance.Helper);
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