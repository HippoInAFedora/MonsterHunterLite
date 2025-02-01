using RoR2;
using UnityEngine.Networking;
using UnityEngine;
using EntityStates;
using MonsterHunterMod.Survivors.Glaive;
using System.Linq;
using MonsterHunterMod.Survivors.Gunlance;

namespace MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates
{
    internal class WyrmStakeBlast: BaseState
    {
        public static float baseDuration = 2.25f;

        public static float duration;

        public static float maxFireDuration = 2f;

        public static float blastAttackRadius = 1f;

        public static float blastAttackRadiusLast = 5f;

        public static float blastAttackDamageCoefficient = .25f;

        public static float blastAttackProcCoefficient = .25f;

        public GameObject attacker;

        public CharacterBody attackerBody;

        private float stopwatch = 0f;

        public override void OnEnter()
        {
            base.OnEnter();         
            duration = baseDuration;
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
            stopwatch += Time.fixedDeltaTime;
            if (stopwatch > maxFireDuration / 8)
            {
                Detonate();
                stopwatch = 0f;
            }
            if (base.fixedAge >= duration && base.isAuthority)
            {
                DetonateLast();
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
                    position = base.characterBody.corePosition + UnityEngine.Random.insideUnitSphere,
                    attacker = attacker,
                    crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master),
                    baseDamage = attackerBody.damage * blastAttackDamageCoefficient,
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

        public void DetonateLast()
        {
            if (NetworkServer.active)
            {
                BlastAttack obj = new BlastAttack
                {
                    radius = blastAttackRadiusLast,
                    procCoefficient = 1,
                    position = base.characterBody.corePosition + UnityEngine.Random.insideUnitSphere,
                    attacker = attacker,
                    crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master),
                    baseDamage = attackerBody.damage * GunlanceStaticValues.wyrmStake,
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
