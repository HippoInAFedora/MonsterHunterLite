using RoR2;
using UnityEngine.Networking;
using UnityEngine;
using MonsterHunterMod.Survivors.Glaive;
using MonsterHunterMod.Characters.Survivors.Glaive.Content;
using R2API;
using EntityStates;

namespace MonsterHunterMod.Characters.Survivors.Glaive.SkillStates
{
    internal class VaultState : GlaiveCharacterMain
    {

        public static GameObject blinkPrefab = null;

        public static float normalJumpPower;

        public static float duration = .4f;

        public static string beginSoundString = null;

        public static string endSoundString = null;

        public static float blastAttackRadius = 5f;

        public static float blastAttackDamageCoefficient = 3f;

        public static float blastAttackProcCoefficient = 1f;

        public static float blastAttackForce = 300f;

        private Vector3 flyVector = Vector3.zero;

        private Transform modelTransform;

        private CharacterModel characterModel;

        private HurtBoxGroup hurtboxGroup;

        private Vector3 blastPosition;

        private float horizontalBonus = 1f;
        private float verticalBonus = 1f;

        public override void OnEnter()
        {
            base.OnEnter();
            if (!base.isGrounded && base.isAuthority)
            {
                base.characterMotor.airControl = 1f;
                horizontalBonus = 2.85f;
                verticalBonus = .55f;
                base.skillLocator.utility.cooldownScale = .6f;
            }
            normalJumpPower = base.characterBody.jumpPower;
            Util.PlaySound(beginSoundString, base.gameObject);
            modelTransform = GetModelTransform();
            flyVector = Vector3.up;
            //CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
            base.characterMotor.Motor.ForceUnground();
            base.characterMotor.velocity = Vector3.zero;
            if (base.isAuthority)
            {
                blastPosition = base.characterBody.corePosition;
                BlastAttack obj = new BlastAttack
                {
                    radius = blastAttackRadius,
                    procCoefficient = blastAttackProcCoefficient,
                    position = blastPosition,
                    attacker = base.gameObject,
                    crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master),
                    baseDamage = base.characterBody.damage * blastAttackDamageCoefficient,
                    falloffModel = BlastAttack.FalloffModel.None,
                    baseForce = 0f,
                    damageType = new DamageTypeCombo(DamageTypeCombo.Generic, DamageTypeExtended.Generic, DamageSource.Utility)
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
                base.characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, .5f);            
            }
            ProcessJump();
        }

        public override void ProcessJump()
        {
            base.ProcessJump();
            if (base.characterBody)
            {
                base.characterBody.jumpPower = 25f;
            }
            if (base.characterMotor)
            {
                base.characterMotor.jumpCount = 0;
            }          
            if (!isGrounded)
            {             
                ApplyJumpVelocity(base.characterMotor, base.characterBody, horizontalBonus, verticalBonus);
            }
            jumpInputReceived = true;
            jumpInputReceived = false;

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
            if (base.fixedAge >= duration && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            if (NetworkServer.active && base.characterBody.GetBuffCount(GlaiveBuffs.airborneDamageBuff) < 3)
            {
                base.characterBody.AddBuff(GlaiveBuffs.airborneDamageBuff);
            }
            if (!outer.destroying)
            {
                Util.PlaySound(endSoundString, base.gameObject);
            }
            if (base.characterMotor)
            {
                base.characterMotor.jumpCount = 0;
                base.characterMotor.airControl = .25f;
            }
            if (base.characterBody)
            {
                base.characterBody.jumpPower = normalJumpPower;
            }
            base.OnExit();

        }
    }
}
