using EntityStates;
using RoR2;
using MonsterHunterMod.Characters.Survivors.Gunlance.Components;
using MonsterHunterMod.Survivors.Glaive;
using MonsterHunterMod.Survivors.Gunlance;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using RoR2.Skills;
using System;
using EntityStates.Croco;
using EntityStates.Merc;
using MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates.BlastStates;

namespace MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates
{
    public class Slam : BaseSkillState, ISkillOverrideHandoff
    {
        public static float damageCoefficient = GunlanceStaticValues.slamDamageCoefficient;

        private float duration;

        private float smallhopDelay = .2f;

        private bool isFalling = false;

        public static float maxFallDuration = 0f;

        public static float maxFallSpeed = 300f;

        public static float maxDistance = 30f;

        public static float initialFallSpeed = -2f;

        public static float accelerationY = 100f;

        private bool hasFired;

        private bool hasLanded;

        private float hitPauseTimer;

        private OverlapAttack attack;

        private bool inHitPause;

        private float stopwatch;
        private float timerAfterHasLanded;

        private float durationAfterLanded = 1f;

        private Animator animator;

        private HitStopCachedState hitStopCachedState;

        private float previousAirControl;

        private bool clearOverrides = true;

        public SkillDef secondaryOverrideSkill;

        private SkillStateOverrideData skillOverrideData;

        private GunlanceShellController gunlanceShellController;
        private GunlanceShellController.ShellingInfo shellInfo;
        private string beginSwingSoundString;
        private GameObject swingEffectPrefab = GlaiveAssets.swordSwingEffect;
        private GameObject swingEffectInstance;
        protected EffectManagerHelper _emh_swingEffectInstance;
        protected string swingEffectMuzzleString = "SlamHitbox";
        private Run.FixedTimeStamp meleeAttackStartTime;

        public override void OnEnter()
        {
            base.OnEnter();
            EnsureSkillOverrideState(readyToFire: false);

            gunlanceShellController = base.GetComponent<GunlanceShellController>();
            if (gunlanceShellController)
            {
                shellInfo = gunlanceShellController.ReturnShellingInfo();
            }


            duration = smallhopDelay / (0.75f + 0.25f * attackSpeedStat);
            durationAfterLanded = .5f + .5f * attackSpeedStat;
            hasFired = false;
            hasLanded = false;
            animator = GetModelAnimator();
            base.characterMotor.jumpCount = base.characterBody.maxJumpCount;
            previousAirControl = base.characterMotor.airControl;
            base.characterMotor.airControl = BaseLeap.airControl;
            Vector3 moveVector = GetAimRay().direction;
            if (base.isAuthority)
            {
                if ((bool)base.characterMotor)
                {
                    base.SmallHop(characterMotor, 5f);
                }
            }
            base.characterDirection.moveVector = moveVector;
            base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = GetModelTransform();
            string hitboxString = "SlamGroup";
            if ((bool)modelTransform)
            {
                hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == hitboxString);
            }
            float num = damageCoefficient;
            attack = new OverlapAttack();
            attack.damageType = DamageTypeCombo.GenericPrimary;
            attack.attacker = base.gameObject;
            attack.inflictor = base.gameObject;
            attack.teamIndex = GetTeam();
            attack.damage = num * base.characterBody.damage;
            attack.procCoefficient = 1f;
            attack.hitEffectPrefab = null;
            attack.forceVector = -Vector3.up * 6000f;
            attack.pushAwayForce = 500f;
            attack.hitBoxGroup = hitBoxGroup;
            attack.isCrit = RollCrit();
            //attack.impactSound = "";
        }

        public override void OnExit()
        {
            base.OnExit();
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            base.characterMotor.airControl = previousAirControl;
            if (clearOverrides && skillOverrideData != null)
            {
                skillOverrideData.ClearOverrides();
            }
        }

        public void FireAttack()
        {
            if (!hasFired)
            {
                BeginMeleeAttackEffect();
                hasFired = true;
            }
            if (base.isAuthority)
            {
                Ray aimRay = GetAimRay();
                if (attack.Fire() && !inHitPause)
                {
                    //hitStopCachedState = CreateHitStopCachedState(base.characterMotor, animator, "Whirlwind.playbackRate");
                    hitPauseTimer = 4f * GroundLight.hitPauseDuration / attackSpeedStat;
                    inHitPause = true;
                }
            }
        }

        protected virtual void BeginMeleeAttackEffect()
        {
            if (meleeAttackStartTime != Run.FixedTimeStamp.positiveInfinity)
            {
                return;
            }
            meleeAttackStartTime = Run.FixedTimeStamp.now;
            Util.PlaySound(beginSwingSoundString, base.gameObject);
            if (!swingEffectPrefab)
            {
                return;
            }
            Transform transform = base.GetModelChildLocator().FindChild(swingEffectMuzzleString);
            if ((bool)transform)
            {
                if (!EffectManager.ShouldUsePooledEffect(swingEffectPrefab))
                {
                    swingEffectInstance = UnityEngine.Object.Instantiate(swingEffectPrefab, transform);
                }
                else
                {
                    _emh_swingEffectInstance = EffectManager.GetAndActivatePooledEffect(swingEffectPrefab, transform, inResetLocal: true);
                    swingEffectInstance = _emh_swingEffectInstance.gameObject;
                }
                ScaleParticleSystemDuration component = swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                if ((bool)component)
                {
                    component.newDuration = component.initialDuration;
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            StartAimMode(0.5f);
            stopwatch += Time.fixedDeltaTime;
            if (base.isAuthority && base.fixedAge > smallhopDelay)
            {
                if ((bool)base.characterMotor && !isFalling)
                {
                    base.characterMotor.velocity.y = Mathf.Min(base.characterMotor.velocity.y, 0f - initialFallSpeed);
                    isFalling = true;
                }
                if (isFalling)
                {
                    Vector3 velocity = base.characterMotor.velocity;
                    if (velocity.y > 0f - maxFallSpeed)
                    {
                        velocity.y = Mathf.MoveTowards(velocity.y, 0f - maxFallSpeed, accelerationY * Time.deltaTime);
                    }
                }     
                if (!hasLanded)
                {
                    FireAttack();
                }               
            }
            if (stopwatch >= duration && base.isAuthority && base.characterMotor.isGrounded)
            {
                GroundImpact();
                if (base.characterMotor)
                {
                    base.characterMotor.velocity = Vector3.zero;
                }                
            }
            EnsureSkillOverrideState(hasLanded);
            if (hasLanded)
            {
                timerAfterHasLanded += Time.fixedDeltaTime;
                if (timerAfterHasLanded > durationAfterLanded && base.isAuthority)
                {
                    outer.SetNextStateToMain();
                }
            }
        }

        private void GroundImpact()
        {
            if (!hasLanded)
            {
                hasLanded = true;
                EffectData effectData = new EffectData();
                effectData.origin = base.characterBody.footPosition;
                effectData.scale = 2f;
                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/ParentSlamEffect"), effectData, transmit: true);
            }
        }

        private void EnsureSkillOverrideState(bool readyToFire)
        {
            if (readyToFire ^ (skillOverrideData != null))
            {
                if (readyToFire)
                {
                    GenerateSkillOverrideData();
                }
                else
                {
                    ClearSkillOverrideData();
                }
            }
        }

        private void GenerateSkillOverrideData()
        {
            secondaryOverrideSkill = SkillCatalog.GetSkillDef(SkillCatalog.FindSkillIndexByName("GunlanceFullBurst"));
            if (skillOverrideData == null)
            {
                skillOverrideData = new SkillStateOverrideData(base.characterBody);
                skillOverrideData.secondarySkillOverride = secondaryOverrideSkill;
                skillOverrideData.overrideFullReloadOnAssign = false;
                skillOverrideData.simulateRestockForOverridenSkills = false;
                skillOverrideData.duplicateStock = true;
                skillOverrideData.OverrideSkills(base.skillLocator);
            }
            //base.skillLocator.secondary.SetSkillOverride(this, secondaryOverrideSkill, GenericSkill.SkillOverridePriority.Contextual);
        }

        private void ClearSkillOverrideData()
        {
            if (skillOverrideData != null)
            {
                skillOverrideData.ClearOverrides();
            }
            skillOverrideData = null;
        }

        void ISkillOverrideHandoff.TransferSkillOverride(SkillStateOverrideData skillOverrideData) => this.skillOverrideData = skillOverrideData;

        public override void ModifyNextState(EntityState nextState)
        {
            base.ModifyNextState(nextState);
            if (nextState is ISkillOverrideHandoff skillOverrideHandoff && skillOverrideData != null)
            {
                skillOverrideHandoff.TransferSkillOverride(skillOverrideData);
                clearOverrides = false;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}

