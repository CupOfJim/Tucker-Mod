namespace TuckertheSabotuer.Artifacts
{

    [ArtifactMeta(pools = new ArtifactPool[] { ArtifactPool.Common })]
    public class ArtofWar : Artifact
    {
        public override void Begin(G g, State s, Combat c)
        {
            int playerShield = c.Ship.Get(Enum.Parse<Status>("shield")) + c.Ship.Get(Enum.Parse<Status>("tempShield"));
            if (playerShield == 0)
            {
                new AAddCard
                {
                    card = new Counterattack(),
                    destination = Enum.Parse<CardDestination>("Hand")
                },
            }
        }
    }
}
