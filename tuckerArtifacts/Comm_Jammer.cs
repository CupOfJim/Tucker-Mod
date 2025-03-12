using System.Reflection;
using Nickel;

namespace TuckerTheSaboteur.Artifacts;


public class CommJammer : Artifact, IRegisterableArtifact 
{
    public static void Register(IModHelper helper)
	{
		helper.Content.Artifacts.RegisterArtifact("CommJammer", new()
		{
			ArtifactType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				owner = Main.Instance.TuckerDeck.Deck,
				pools = [ArtifactPool.Common]
			},
			Sprite = helper.Content.Sprites.RegisterSprite(Main.Instance.Package.PackageRoot.GetRelativeFile("sprites/icons/Comm_Jammer.png")).Sprite,
			Name = Main.Instance.AnyLocalizations.Bind(["artifact", "CommJammer", "name"]).Localize,
			Description = Main.Instance.AnyLocalizations.Bind(["artifact", "CommJammer", "description"]).Localize
		});
	}

    public override int ModifyBaseMissileDamage(State state, Combat? combat, bool targetPlayer)
    {
        if (!targetPlayer) return 0;

        return -1;
    }
}
