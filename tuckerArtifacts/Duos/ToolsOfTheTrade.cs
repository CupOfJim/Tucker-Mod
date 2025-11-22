using System.Collections.Generic;
using System.Reflection;
using Nickel;
using TuckerTheSaboteur;
using TuckerTheSaboteur.actions;

namespace TuckerMod.tuckerArtifacts.Duos;

public class ToolsOfTheTrade : Artifact, IRegisterableArtifact
{
	public static void Register(IModHelper helper)
	{
		if (Main.Instance.DuoArtifactsApi == null) return;

		helper.Content.Artifacts.RegisterArtifact("ToolsOfTheTrade", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Main.Instance.DuoArtifactsApi.DuoArtifactVanillaDeck,
			},
			Sprite = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/icons/duos/ToolsOfTheTrade.png")).Sprite,
			Name = Main.Instance.AnyLocalizations.Bind(["artifact", "duo", "ToolsOfTheTrade", "name"]).Localize,
			Description = Main.Instance.AnyLocalizations.Bind(["artifact", "duo", "ToolsOfTheTrade", "description"]).Localize
		});
		Main.Instance.DuoArtifactsApi.RegisterDuoArtifact<ToolsOfTheTrade>([Deck.dizzy, Main.Instance.TuckerDeck.Deck]);
	}

	public override List<Tooltip>? GetExtraTooltips() => new AShieldSteal {
		amount = 1,

	}.GetTooltips(DB.fakeState);
}