using RoR2;
using UnityEngine.Networking;
using UnityEngine;
using MonsterHunterMod.Survivors.Glaive;
using EntityStates;
using MonsterHunterMod.Characters.Survivors.Glaive.Content;
using R2API;

namespace MonsterHunterMod.Characters.Survivors.Glaive.SkillStates.old
{
    internal class CrashEnd : BaseState
    {
        public static GameObject blinkPrefab = null;

        public float airborneDamage;

        public static float baseDuration = 0.2f;

        public static float duration;

        public static string beginSoundString = null;

        public static string endSoundString = null;

        public static float blastAttackRadius = 15f;

        public static float blastAttackDamageCoefficient = 15f;

        public static float blastAttackProcCoefficient = 1f;

        public static float blastAttackForce = 500f;

        private Vector3 fallVector = Vector3.zero;

        private Transform modelTransform;

        private Vector3 blastPosition;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            Util.PlaySound(beginSoundString, gameObject);
            modelTransform = GetModelTransform();
            //CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
            characterMotor.velocity = Vector3.zero;
            if (isAuthority)
            {
                blastPosition = characterBody.corePosition;
                BlastAttack obj = new BlastAttack
                {
                    radius = blastAttackRadius,
                    procCoefficient = blastAttackProcCoefficient,
                    position = blastPosition,
                    attacker = gameObject,
                    crit = Util.CheckRoll(characterBody.crit, characterBody.master),
                    baseDamage = characterBody.damage * blastAttackDamageCoefficient * airborneDamage,
                    falloffModel = BlastAttack.FalloffModel.None,
                    baseForce = blastAttackForce
                };
                obj.AddModdedDamageType(GlaiveDamageTypes.glaiveHit);
                obj.teamIndex = TeamComponent.GetObjectTeam(obj.attacker);
                obj.attackerFiltering = AttackerFiltering.NeverHitSelf;
                obj.Fire();
                EffectData data = new EffectData();
                data.scale = obj.radius * 2;
                data.origin = obj.position;
                EffectManager.SpawnEffect(GlaiveAssets.circleSlashEffect, data, true);
            }
            if (NetworkServer.active)
            {
                characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, duration);
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

        private void CreateBlinkEffect(Vector3 origin)
        {
            EffectData effectData = new EffectData();
            effectData.rotation = Util.QuaternionSafeLookRotation(fallVector);
            effectData.origin = origin;
            EffectManager.SpawnEffect(blinkPrefab, effectData, transmit: false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            if (!outer.destroying)
            {
                Util.PlaySound(endSoundString, gameObject);
            }
            base.OnExit();
        }
    }
}
