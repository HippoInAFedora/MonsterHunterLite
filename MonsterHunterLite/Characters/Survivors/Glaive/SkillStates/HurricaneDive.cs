using RoR2;
using UnityEngine.Networking;
using UnityEngine;
using MonsterHunterMod.Survivors.Glaive;
using EntityStates;
using R2API;
using MonsterHunterMod.Characters.Survivors.Glaive.Content;
using System.Linq;
using System;

namespace MonsterHunterMod.Characters.Survivors.Glaive.SkillStates
{
    internal class HurricaneDive : BaseState
    {
        protected CharacterBody body;

        private float stopwatch;

        private float tickRate;

        private float tick;

        bool detonateNextFrame;

        private float smallhopDelay = .2f;

        private bool isFalling = false;

        public static float maxFallDuration = 0f;

        public static float maxFallSpeed = 45f;

        public static float maxDistance = 30f;

        public static float initialFallSpeed = -5f;

        public static float accelerationY = 25f;

        public static float radius = GlaiveStaticValues.hurricaneBlastRadius;

        public static float airDamage = GlaiveStaticValues.hurricaneDiveDamageCoefficient;

        public static float groundDamage = GlaiveStaticValues.hurricaneBlastDamageCoefficient;

        private CharacterMotor onHitGroundProvider;

        public static float airborneDamage;

        public override void OnEnter()
        {
            base.OnEnter();
            body = base.characterBody;
            int num = 0;
            tick = .2f/base.characterBody.attackSpeed;
            if (NetworkServer.active)
            {
                num = characterBody.GetBuffCount(GlaiveBuffs.airborneDamageBuff);
                characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
            airborneDamage = 1f + .2f * num;
            if (base.isAuthority)
            {
                base.characterMotor.onMovementHit += OnMovementHit;
                if ((bool)base.characterMotor)
                {
                    base.SmallHop(characterMotor, 15f);
                }
            }
            
        }

        private void OnMovementHit(ref CharacterMotor.MovementHitInfo movementHitInfo)
        {
            detonateNextFrame = true;
        }


        public override void OnExit()
        {
            if (base.isAuthority) 
            {
                base.characterMotor.onMovementHit -= OnMovementHit;
            }
            if (NetworkServer.active)
            {
                characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
            base.OnExit();
        }

        private void OnMotorHitGroundAuthority(ref CharacterMotor.HitGroundInfo hitGroundInfo)
        {
            DoStompExplosionAuthority();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && base.fixedAge > smallhopDelay)
            {               
                if ((bool)base.characterMotor && !isFalling)
                {
                    base.characterMotor.velocity.y = Mathf.Min(base.characterMotor.velocity.y, 0f - initialFallSpeed);
                    isFalling = true;
                }
                FixedUpdateAuthority();
            }
        }

        private void FixedUpdateAuthority()
        {
            stopwatch += Time.deltaTime;
            tickRate += Time.fixedDeltaTime;
            if (tickRate > tick)
            {
                TickBlast();
                tickRate = 0;
            }
            if (detonateNextFrame || base.characterMotor.Motor.GroundingStatus.IsStableOnGround)
            {
                DoStompExplosionAuthority();
                outer.SetNextStateToMain();
            }
            //else if (stopwatch >= maxFallDuration)
            //{
            //    outer.SetNextStateToMain();
            //}
            else if (base.inputBank.jump.down)
            {
                base.characterMotor.moveDirection += base.inputBank.moveVector * attackSpeedStat;
                base.characterMotor.Jump(1, 1);
                base.skillLocator.special.RunRecharge(base.skillLocator.special.CalculateFinalRechargeInterval() / 2);
                outer.SetNextStateToMain();
            }
            else
            {
                if (!base.characterMotor)
                {
                    return;
                }
                Vector3 velocity = base.characterMotor.velocity;
                if (velocity.y > 0f - maxFallSpeed)
                {
                    velocity.y = Mathf.MoveTowards(velocity.y, 0f - maxFallSpeed, accelerationY * Time.deltaTime);
                }
                base.characterMotor.velocity = velocity + base.inputBank.moveVector / 2;
            }
        }

        private void TickBlast()
        {
            BlastAttack obj = new BlastAttack
            {
                radius = GlaiveStaticValues.hurricaneDiveRadius,
                procCoefficient = 1f,
                position = base.characterBody.corePosition,
                attacker = gameObject,
                canRejectForce = true,
                crit = Util.CheckRoll(characterBody.crit, characterBody.master),
                baseDamage = characterBody.damage * GlaiveStaticValues.hurricaneDiveDamageCoefficient * airborneDamage,
                falloffModel = BlastAttack.FalloffModel.None,
                baseForce = 0f,
                bonusForce = Vector3.zero,
                damageType = new DamageTypeCombo(DamageTypeCombo.Generic, DamageTypeExtended.Generic, DamageSource.Special)             
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

        private void DoStompExplosionAuthority()
        {
            base.characterMotor.velocity = Vector3.zero;
            BlastAttack obj = new BlastAttack
            {
                attacker = base.gameObject,
                inflictor = base.gameObject
            };
            obj.teamIndex = TeamComponent.GetObjectTeam(obj.attacker);
            obj.position = base.characterBody.footPosition;
            obj.procCoefficient = 1f;
            obj.radius = radius;
            obj.canRejectForce = true;
            obj.baseForce = 0f;
            obj.bonusForce = Vector3.zero;
            obj.baseDamage = base.characterBody.damage * groundDamage;
            obj.falloffModel = BlastAttack.FalloffModel.None;
            obj.crit = base.RollCrit();
            obj.damageColorIndex = DamageColorIndex.Default;
            obj.attackerFiltering = AttackerFiltering.NeverHitSelf;
            obj.damageType = DamageTypeCombo.GenericSpecial;
            obj.AddModdedDamageType(GlaiveDamageTypes.glaiveHit);
            obj.Fire();
            EffectData effectData = new EffectData();
            effectData.origin = base.characterBody.footPosition;
            effectData.scale = radius * 2;
            EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/BootShockwave"), effectData, transmit: true);
        }
    }
}
