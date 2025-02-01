using MonsterHunterMod.Characters.Survivors.Glaive.Components;
using MonsterHunterMod.Survivors.Glaive;
using RoR2;
using RoR2.Orbs;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace MonsterHunterMod.Characters.Survivors.Glaive.Content.Orbs
{
    internal class GlaiveOrbDelay : DelayedHitOrb
    {
        public bool noBuff;

        public BuffDef debuff = null;

        public float speedThruOrbs;

        public List<string> whiteFlagNames = GlaiveSurvivor.whiteFlagNames;

        public List<string> redFlagNames = GlaiveSurvivor.redFlagNames;

        public List<string> orangeFlagNames = GlaiveSurvivor.orangeFlagNames;

        public string targetName;
        public override void Begin()
        {
            delay = .2f;
            if (attacker.gameObject.TryGetComponent(out CharacterBody body))
            {
                float attackSpeed = body.attackSpeed;
                delay = delay / attackSpeed;
            }
            base.Begin();
        }

        public override GameObject GetOrbEffect()
        {
            return GlaiveAssets.orbBugBug;
        }

        public override void OnArrival()
        {
            base.OnArrival();
            if (debuff != null && NetworkServer.active)
            {
                
                if (debuff == GlaiveBuffs.blastOnStrike)
                {
                    bool flag = target.gameObject.GetComponent<BlastOnStrikeBuffBehavior>();
                    if (!flag)
                    {
                        target.gameObject.AddComponent<BlastOnStrikeBuffBehavior>().body = target.healthComponent.body;
                    }
                    target.gameObject.GetComponent<BlastOnStrikeBuffBehavior>().enabled = true;
                    target.healthComponent.body.AddBuff(debuff);
                }
                else
                {
                    target.healthComponent.body.AddBuff(debuff);
                }


            }
            nextOrb = ReturningOrbType();
            HurtBox hurtBox = attacker.GetComponent<CharacterBody>().mainHurtBox;
            if (hurtBox)
            {
                Transform transform = target.transform;
                nextOrb.origin = transform.position;
                nextOrb.target = hurtBox;
                OrbManager.instance.AddOrb(nextOrb);
            }    
        }
        //All Status Orbs inherit from NoBuffBug just for a very small simplification, a la no need for speed and scale variables through inheritance. A poor choice.
        private Orb RandomizedOrb()
        {
            int num = Random.RandomRangeInt(0, 3);
            if (num == 0)
            {
                return new WhiteBugOrb{
                speed = speedThruOrbs};
                
            }
            if (num == 1)
            {
                return new RedBugOrb{speed=speedThruOrbs};
            }
            else
            {
                return new OrangeBugOrb{speed=speedThruOrbs};
            }
        }

        private Orb ChooseOrb(bool[] bodyFlags) 
        {
            if (bodyFlags[0] == true)
            {
                return new WhiteBugOrb{ speed = speedThruOrbs };
            }
            else if (bodyFlags[1] == true)
            {
                return new RedBugOrb{ speed = speedThruOrbs };
            }
            else if (bodyFlags[2] == true)
            {
                return new OrangeBugOrb{ speed = speedThruOrbs };
            }
            else return RandomizedOrb();
        }


        public Orb ReturningOrbType()
        {
            if (noBuff)
            {
                return new NoBuffBug{ speed = speedThruOrbs };
            }
            targetName = target.healthComponent.body.name;
            CharacterBody body = attacker.GetComponent<CharacterBody>();
            bool[] bodyFlags = new bool[3];
            bodyFlags[0] = false;
            bodyFlags[1] = false;
            bodyFlags[2] = false;
            if (body)
            {
                if (!body.HasBuff(GlaiveBuffs.whiteBugBuff))
                {
                    bodyFlags[0] = true;
                }
                if (!body.HasBuff(GlaiveBuffs.redBugBuff))
                {
                    bodyFlags[1] = true;
                }
                if (!body.HasBuff(GlaiveBuffs.orangeBugBuff))
                {
                    bodyFlags[2] = true;
                }              
            }
            bool flag  = target.healthComponent.body.HasBuff(DLC1Content.Buffs.EliteVoid) ? true : false;
            if (flag)
            {
                return RandomizedOrb();
            }

            bool flag2 = target.healthComponent.body.isBoss || target.healthComponent.body.isChampion ? true : false;
            if (flag2)
            {
                return ChooseOrb(bodyFlags);
            }

            bool flag3 = target.healthComponent.body.isElite ? true : false;
            if (flag3)
            {
                if (whiteFlagNames.Contains(targetName) && bodyFlags[0])
                {
                    return new WhiteBugOrb { speed = speedThruOrbs };
                }
                if (redFlagNames.Contains(targetName) && bodyFlags[1])
                {
                    return new RedBugOrb { speed = speedThruOrbs };
                }
                if (orangeFlagNames.Contains(targetName) && bodyFlags[2])
                {
                    return new OrangeBugOrb { speed = speedThruOrbs };
                }
                return(ChooseOrb(bodyFlags));
            }

            if (whiteFlagNames.Contains(targetName))
            {
                return new WhiteBugOrb{ speed = speedThruOrbs };
            }
            if (redFlagNames.Contains(targetName))
            {
                return new RedBugOrb{ speed = speedThruOrbs };
            }
            if (orangeFlagNames.Contains(targetName))
            {
                return new OrangeBugOrb{ speed = speedThruOrbs };
            }

            Debug.LogWarning("[MonsterHunterMod] enemy has no set buff or is modded.");
            return ChooseOrb(bodyFlags);          
        }
    }
}
