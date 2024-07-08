using FMOD;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuckertheSabotuer.Artifacts;

namespace TuckerTheSaboteur.actions
{
    [HarmonyPatch]
    public class ABluntAttack : AAttack
    {
        static bool blockStunSource = false;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Ship), nameof(Ship.Get))]
        public static void HarmonyPostfix_BluntAttackBlockStunCharge(ref int __result, Status name)
        {
            if (!blockStunSource) return;
            if (name != Enum.Parse<Status>("stunCharge")) return;
            __result = 0;
        }

        private int? CopypastedGetFromX(State s, Combat c)
        {
            if (fromX.HasValue)
            {
                return fromX;
            }
            int num = (targetPlayer ? c.otherShip : s.ship).parts.FindIndex((Part p) => p.type == PType.cannon && p.active);
            if (num != -1)
            {
                return num;
            }
            return null;
        }

        public override void Begin(G g, State s, Combat c)
        {
            int? num = CopypastedGetFromX(s, c);
            RaycastResult raycastResult = CombatUtils.RaycastFromShipLocal(s, c, num.Value, targetPlayer);
            
            bool targetHasShield = false;
            if (raycastResult?.hitShip == true) targetHasShield = 0 < c.otherShip.Get(Enum.Parse<Status>("shield")) + c.otherShip.Get(Enum.Parse<Status>("tempShield"));
            if (raycastResult?.hitDrone == true) targetHasShield = c.stuff[raycastResult.worldX].Invincible() || c.stuff[raycastResult.worldX].bubbleShield;

            if (targetHasShield)
            {
                var ship = targetPlayer ? c.otherShip : s.ship;
                var ship2 = targetPlayer ? s.ship : c.otherShip;

                // handle hitting an invincible drone
                if (raycastResult?.hitDrone == true && c.stuff[raycastResult.worldX].Invincible()) c.QueueImmediate(c.stuff[raycastResult.worldX].GetActionsOnShotWhileInvincible(s, c, !targetPlayer, damage));

                if (!isBeam && !targetPlayer && !fromDroneX.HasValue)
                {
                    Input.Rumble(0.5);
                }
                if (!isBeam)
                {
                    g.state.storyVars.playerShotJustMissed = true;
                    g.state.storyVars.playerShotWasFromStrafe = storyFromStrafe;
                    g.state.storyVars.playerShotWasFromPayback = storyFromPayback;

                    if (!fromDroneX.HasValue)
                    {
                        foreach (Artifact item9 in s.EnumerateAllArtifacts())
                        {
                            item9.OnPlayerAttack(s, c);
                        }
                    }
                    if (raycastResult.hitShip)
                    {
                        if (c.otherShip.ai != null)
                        {
                            c.otherShip.ai.OnHitByAttack(s, c, raycastResult.worldX, this);
                        }
                        foreach (Artifact item10 in s.EnumerateAllArtifacts())
                        {
                            item10.OnEnemyGetHit(s, c, ship2.GetPartAtWorldX(raycastResult.worldX));
                        }
                    }
                    if (!raycastResult.hitShip && !raycastResult.hitDrone && !fromDroneX.HasValue)
                    {
                        foreach (Artifact item11 in s.EnumerateAllArtifacts())
                        {
                            item11.OnEnemyDodgePlayerAttack(s, c);
                        }
                    }
                    if (!raycastResult.hitShip && !raycastResult.hitDrone)
                    {
                        bool flag3 = false;
                        for (int i = -1; i <= 1; i += 2)
                        {
                            if (CombatUtils.RaycastGlobal(c, ship, fromDrone: true, raycastResult.worldX + i).hitShip)
                            {
                                flag3 = true;
                            }
                        }
                        if (flag3)
                        {
                            foreach (Artifact item12 in s.EnumerateAllArtifacts())
                            {
                                item12.OnEnemyDodgePlayerAttackByOneTile(s, c);
                            }
                        }
                    }
                }
                if (ship.hull <= 0 && onKillActions != null)
                {
                    List<CardAction> list = Mutil.DeepCopy(onKillActions);
                    foreach (CardAction item13 in list)
                    {
                        item13.canRunAfterKill = true;
                    }
                    c.QueueImmediate(list);
                }
                                
                Part partAtLocalX = ship.GetPartAtLocalX(num.Value);
                if (partAtLocalX != null)
                {
                    partAtLocalX.pulse = 1.0;
                }

                // laser effect
                EffectSpawner.Cannon(
                    g, 
                    targetPlayer, 
                    raycastResult, 
                    new DamageDone
                    {
                        hullAmt = 0,
                        hitHull = false,
                        hitShield = true,
                        poppedShield = false
                    }, 
                    isBeam
                );
            }
            else
            {
                var ownedBrick = g.state.EnumerateAllArtifacts().Where((Artifact a) => a.GetType() == typeof(Brick)).FirstOrDefault() as Brick;
                if (ownedBrick != null)
                {
                    ownedBrick.Pulse();
                    base.damage += 1; // not using GetDamage here, since it was already used. If we used it again, it'd apply overdrive (and similar effects) twice
                }

                base.Begin(g, s, c);
            }

            blockStunSource = false;
        }

        // Lifted directly from decompiled game code
        // Yes it is called that in the source
        // I love the devs
        public static bool DoWeHaveCannonsThough(State s)
        {
            foreach (Part part in s.ship.parts)
            {
                if (part.type == PType.cannon)
                {
                    return true;
                }
            }
            if (s.route is Combat combat)
            {
                foreach (StuffBase value in combat.stuff.Values)
                {
                    if (value is JupiterDrone)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private Spr GetSpr() { return (Spr)MainManifest.sprites["icons/Blunt_Attack"].Id; }
        private Spr GetSprFail() { return (Spr)MainManifest.sprites["icons/Blunt_Attack_Fail"].Id; }

        public override Icon? GetIcon(State s)
        {
            int buff = 0;
            var ownedBrick = s.EnumerateAllArtifacts().Where((Artifact a) => a.GetType() == typeof(Brick)).FirstOrDefault() as Brick;
            if (ownedBrick != null)
            {
                buff = 1;
            }

            if (DoWeHaveCannonsThough(s))
            {
                return new Icon(GetSpr(), damage+buff, Colors.redd);
            }

            return new Icon(GetSprFail(), damage+buff, Colors.redd);
        }

        public override List<Tooltip> GetTooltips(State s)
        {
            int buff = 0;
            var ownedBrick = s.EnumerateAllArtifacts().Where((Artifact a) => a.GetType() == typeof(Brick)).FirstOrDefault() as Brick;
            if (ownedBrick != null)
            {
                buff = 1;
            }

            return new()
            {
                new Shockah.Kokoro.CustomTTGlossary
                (
                    Shockah.Kokoro.CustomTTGlossary.GlossaryType.action,
                    () => GetSpr(),
                    () => "Blunt Attack",
                    () => "Deals {0} damage if the enemy has no shields (or temp shields) and is unarmored. Otherwise, deals 0.",
                    new List<Func<object>>()
                    {
                        () => ""+(damage+buff),
                    }
                )
            };
        }
    }
}
