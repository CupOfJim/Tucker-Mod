using TuckerTheSaboteur.cards;

namespace TuckertheSabotuer.Artifacts
{

    [ArtifactMeta(pools = new ArtifactPool[] { ArtifactPool.Common })]
    public class ArtofWar : Artifact
    {
        public override void OnTurnStart(State s, Combat c)
        {
            int playerShield =  s.ship.Get(Enum.Parse<Status>("shield")) + s.ship.Get(Enum.Parse<Status>("tempShield"));
            if (playerShield == 0)
            {
                c.QueueImmediate(new AAddCard()
                {
                    card = new Counterattack(),
                    destination = Enum.Parse<CardDestination>("Hand")
                });
            }
        }
    }
}
