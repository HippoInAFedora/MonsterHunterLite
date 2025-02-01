using MonsterHunterMod.Characters.Survivors.Glaive.Components;
using MonsterHunterMod.Survivors.Glaive;
using RoR2;
using RoR2.Orbs;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;
using static RoR2.CharacterBody;
using static RoR2.Orbs.LightningOrb;

namespace MonsterHunterMod.Characters.Survivors.Glaive.Content.Orbs
{
    internal class GlaiveOrb : GenericDamageOrb
    {
        public bool noBuff;

        public int previousBugFlag = 0;

        public BuffDef debuff = null;

        public float speedThruOrbs;

        public List<string> whiteFlagNames = GlaiveSurvivor.whiteFlagNames;

        public List<string> redFlagNames = GlaiveSurvivor.redFlagNames;

        public List<string> orangeFlagNames = GlaiveSurvivor.orangeFlagNames;


        public int targetsToFindPerBounce = 1;

        public int bouncesRemaining = 0;

        public float range = 50f;

        private bool canBounceOnSameTarget;

        private BullseyeSearch search;

        public string targetName;
        public override void Begin()
        {
            base.Begin();
            speedThruOrbs = speed;
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
            if (bouncesRemaining > 0)
            {
                InstantiateBounce();
            }
        }

        private void InstantiateBounce()
        {
            canBounceOnSameTarget = target.healthComponent.body.isElite || target.healthComponent.body.isChampion || target.healthComponent.body.isBoss;
            HurtBox hurtBox = (canBounceOnSameTarget && target.healthComponent.alive) ? target : PickNextTarget(target.transform.position);        
            if (hurtBox != null)
            {
                GlaiveOrb glaiveOrb = new GlaiveOrb();
                glaiveOrb.search = search;
                glaiveOrb.origin = target.transform.position;
                glaiveOrb.target = hurtBox;
                glaiveOrb.attacker = attacker;
                glaiveOrb.teamIndex = teamIndex;
                glaiveOrb.damageValue = damageValue;
                glaiveOrb.bouncesRemaining = bouncesRemaining - 1;
                glaiveOrb.isCrit = isCrit;
                glaiveOrb.procChainMask = procChainMask;
                glaiveOrb.procCoefficient = procCoefficient;
                glaiveOrb.damageColorIndex = damageColorIndex;
                glaiveOrb.speed = speedThruOrbs;
                glaiveOrb.range = range;
                glaiveOrb.damageType = damageType;
                glaiveOrb.previousBugFlag = previousBugFlag;
                OrbManager.instance.AddOrb(glaiveOrb);
            }

        }
        //All Status Orbs inherit from NoBuffBug just for a very small simplification, a la no need for speed and scale variables through inheritance. A poor choice.
        private Orb RandomizedOrb()
        {
            int num = Random.RandomRangeInt(0, 3);
            if (num == 0)
            {
                previousBugFlag = 1;
                return new WhiteBugOrb
                {
                    speed = speedThruOrbs
                };

            }
            if (num == 1)
            {
                previousBugFlag = 2;
                return new RedBugOrb { speed = speedThruOrbs };
            }
            else
            {
                previousBugFlag = 3;
                return new OrangeBugOrb { speed = speedThruOrbs };
            }
        }

        private Orb ChooseOrb(bool[] bodyFlags)
        {
            if (bodyFlags[0] == true)
            {
                previousBugFlag = 1;
                return new WhiteBugOrb { speed = speedThruOrbs };
            }
            else if (bodyFlags[1] == true)
            {
                previousBugFlag = 2;
                return new RedBugOrb { speed = speedThruOrbs };
            }
            else if (bodyFlags[2] == true)
            {
                previousBugFlag = 3;
                return new OrangeBugOrb { speed = speedThruOrbs };
            }
            else return RandomizedOrb();
        }

        private bool[] GetBodyFlags()
        {
            targetName = target.healthComponent.body.name;
            CharacterBody body = attacker.GetComponent<CharacterBody>();
            bool[] bodyFlags = new bool[4];
            bodyFlags[0] = false;
            bodyFlags[1] = false;
            bodyFlags[2] = false;
            if (body)
            {
                if (!body.HasBuff(GlaiveBuffs.whiteBugBuff) && previousBugFlag != 1)
                {
                    bodyFlags[0] = true;
                }
                if (!body.HasBuff(GlaiveBuffs.redBugBuff) && previousBugFlag != 2)
                {
                    bodyFlags[1] = true;
                }
                if (!body.HasBuff(GlaiveBuffs.orangeBugBuff) && previousBugFlag != 3)
                {
                    bodyFlags[2] = true;
                }
            }
            return bodyFlags;
        }

        public Orb ReturningOrbType()
        {
            if (noBuff)
            {
                return new NoBuffBug { speed = speedThruOrbs };
            }
            bool[] bodyFlags = GetBodyFlags();
            bool flag = target.healthComponent.body.HasBuff(DLC1Content.Buffs.EliteVoid) ? true : false;
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
                    previousBugFlag = 1;
                    return new WhiteBugOrb { speed = speedThruOrbs };
                }
                if (redFlagNames.Contains(targetName) && bodyFlags[1])
                {
                    previousBugFlag = 2;
                    return new RedBugOrb { speed = speedThruOrbs };
                }
                if (orangeFlagNames.Contains(targetName) && bodyFlags[2])
                {
                    previousBugFlag = 3;
                    return new OrangeBugOrb { speed = speedThruOrbs };
                }
                return (ChooseOrb(bodyFlags));
            }

            if (whiteFlagNames.Contains(targetName))
            {
                previousBugFlag = 1;
                return new WhiteBugOrb { speed = speedThruOrbs };
            }
            if (redFlagNames.Contains(targetName))
            {
                previousBugFlag = 2;
                return new RedBugOrb { speed = speedThruOrbs };
            }
            if (orangeFlagNames.Contains(targetName))
            {
                previousBugFlag = 3;
                return new OrangeBugOrb { speed = speedThruOrbs };
            }

            Debug.LogWarning("[MonsterHunterMod] enemy has no set buff or is modded.");
            return ChooseOrb(bodyFlags);
        }


        public HurtBox PickNextTarget(Vector3 position)
        {
            bool[] bodyFlags = GetBodyFlags();
            search = new BullseyeSearch();
            search.Reset();
            search.searchOrigin = position;
            search.searchDirection = Vector3.zero;
            search.teamMaskFilter = TeamMask.allButNeutral;
            search.teamMaskFilter.RemoveTeam(teamIndex);
            search.filterByLoS = false;
            search.sortMode = BullseyeSearch.SortMode.Distance;
            search.filterByDistinctEntity = true;
            search.maxDistanceFilter = range;
            search.RefreshCandidates();
            bool flag = false;
            HurtBox[] hurtBox = search.GetResults().ToArray();
            HurtBox bufferedFirstBounce = null;
            if (hurtBox.Length > 1)
            {
                bufferedFirstBounce = hurtBox[1];
            }           
            for (int i = 0; i < hurtBox.Length && flag == false; i++)
            {
                if (hurtBox[i].healthComponent)
                {
                    string name = hurtBox[i].healthComponent.body.name;
                    bool hasAllBuffs = hurtBox[i].healthComponent.body.isElite || hurtBox[i].healthComponent.body.isChampion || hurtBox[i].healthComponent.body.isBoss;
                    if (!hasAllBuffs && !bodyFlags.All(x => false))
                    {
                        name = hurtBox[i].healthComponent.body.name;
                        if (bodyFlags[0] != true && whiteFlagNames.Contains(name))
                        {
                            hurtBox[i] = null;
                        }
                        if (bodyFlags[1] != true && redFlagNames.Contains(name))
                        {
                            hurtBox[i] = null;
                        }
                        if (bodyFlags[2] != true && orangeFlagNames.Contains(name))
                        {
                            hurtBox[i] = null;
                        }
                    }                   
                    if (hurtBox[i] != null)
                    {
                        flag = true;
                        return hurtBox[i];
                    }
                }
            }
            if (bufferedFirstBounce)
            {
                return bufferedFirstBounce;
            }
            return null;
        }

        //public bool noBuff = false;
        //public BuffDef debuff = null;
        //public override void Begin()
        //{
        //    base.Begin();
        //}

        //public override GameObject GetOrbEffect()
        //{
        //    return GlaiveAssets.orbBugBug;
        //}

        //public override void OnArrival()
        //{
        //    base.OnArrival();
        //    GlaiveOrbDelay delayOrb = new GlaiveOrbDelay();
        //    //delayOrb.damageValue = base.characterBody.damage * damageCoefficient;
        //    delayOrb.isCrit = isCrit;
        //    delayOrb.teamIndex = TeamComponent.GetObjectTeam(attacker.gameObject);
        //    delayOrb.attacker = attacker;
        //    delayOrb.procCoefficient = procCoefficient;
        //    delayOrb.noBuff = noBuff;
        //    nextOrb = delayOrb;
        //    delayOrb.debuff = debuff;
        //    delayOrb.speedThruOrbs = speed;
        //    HurtBox hurtBox = target;
        //    if (hurtBox)
        //    {
        //        Transform transform = target.transform;
        //        nextOrb.origin = transform.position;
        //        nextOrb.target = hurtBox;
        //        OrbManager.instance.AddOrb(nextOrb);
        //    }          
        //}
    }
}
