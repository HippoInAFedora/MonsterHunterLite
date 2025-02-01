using RoR2;
using UnityEngine.Networking;
using UnityEngine;
using MonsterHunterMod.Survivors.Glaive;
using EntityStates;
using MonsterHunterMod.Characters.Survivors.Glaive.Content;
using R2API;
using MonsterHunterMod.Characters.Survivors.Glaive.Components;

namespace MonsterHunterMod.Characters.Survivors.Glaive.SkillStates
{
    internal class BlastBugDetonate : BaseState
    {
        public static float baseDuration = 0.2f;

        public static float duration;

        public static float blastAttackRadius = 4f;

        public static float blastAttackDamageCoefficient = 1f;

        public static float blastAttackProcCoefficient = 1f;

        private Vector3 blastPosition;

        public GameObject attacker;

        public CharacterBody attackerBody;

        public float[] timer;
        public bool[] flag;

        public override void OnEnter()
        {
            base.OnEnter();         
            timer = new float[6];
            flag = new bool[6];
            for (int i = 0; i < timer.Length; i++)
            {
                timer[i] = .2f * i;
                flag[i] = false;
            }
            duration = baseDuration + (.2f * timer.Length);
            if (base.isAuthority)
            {
                blastPosition = base.characterBody.corePosition;
                
            }
            if (NetworkServer.active)
            {                
                base.characterBody.RemoveBuff(GlaiveBuffs.blastOnStrike);
                //base.characterBody.RemoveBuff(GlaiveBuffs.blastOnStrikeVisualCooldown);             
            }
        }

        //public override void OnSerialize(NetworkWriter writer)
        //{
        //    base.OnSerialize(writer);
        //    writer.Write(blastPosition);
        //}

        //public override void OnDeserialize(NetworkReader reader)
        //{
        //    base.OnDeserialize(reader);
        //    blastPosition = reader.ReadVector3();
        //}

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            for (int i = 0;i < timer.Length; i++)
            {

                if (base.fixedAge > timer[i] && flag[i] == false)
                {
                    Detonate();
                    flag[i] = true;
                }
            }
            if (base.fixedAge >= duration && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public void Detonate()
        {
            if (NetworkServer.active)
            {
                BlastAttack obj = new BlastAttack
                {
                    radius = blastAttackRadius,
                    procCoefficient = blastAttackProcCoefficient,
                    position = blastPosition + UnityEngine.Random.insideUnitSphere,
                    attacker = attacker,
                    crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master),
                    baseDamage = attackerBody.damage * 1f,
                    falloffModel = BlastAttack.FalloffModel.None,
                };
                obj.teamIndex = TeamComponent.GetObjectTeam(obj.attacker);
                obj.attackerFiltering = AttackerFiltering.NeverHitSelf;
                obj.Fire();

                EffectData data = new EffectData();
                data.origin = obj.position;
                data.scale = obj.radius;
                EffectManager.SpawnEffect(GlaiveAssets.blastBugEffect, data, true);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
