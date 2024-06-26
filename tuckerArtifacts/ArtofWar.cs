﻿using TuckerTheSaboteur.cards;

namespace TuckertheSabotuer.Artifacts
{

    [ArtifactMeta(pools = new ArtifactPool[] { ArtifactPool.Common })]
    public class ArtofWar : Artifact
    {
        public int lastTurnShield = 0;
        public override void OnCombatStart(State state, Combat combat)
        {
            lastTurnShield = 0;
        }
        public override void OnCombatEnd(State state)
        {
            lastTurnShield = 0;
        }

        public override void OnTurnStart(State s, Combat c)
        {
            int playerShield = s.ship.Get(Enum.Parse<Status>("shield")) + s.ship.Get(Enum.Parse<Status>("tempShield"));
            if (playerShield == 0 && lastTurnShield != 0)
            {
                c.QueueImmediate(new AAddCard()
                {
                    card = new Counterattack(),
                    destination = Enum.Parse<CardDestination>("Hand")
                });
            }

            lastTurnShield = playerShield;
        }
    }
}
