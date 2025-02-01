using EntityStates;
using UnityEngine;
using System;
using RoR2;
using MonsterHunterMod.Characters.Survivors.Gunlance.Components;
using MonsterHunterMod.Survivors.Gunlance;
using UnityEngine.Networking;
using EntityStates.Merc;
using EntityStates.Croco;
using MonsterHunterMod.Survivors.Glaive;

namespace MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates.BlastStates
{
    public class BlastDash : BaseSkillState
    {
        public static float damageCoefficient = GunlanceStaticValues.blastDashDamageCoefficient;

        private float uninterruptibleDuration;

        private float duration;

        private bool hasFired;

        private float hitPauseTimer;

        private OverlapAttack attack;

        private bool inHitPause;

        private float stopwatch;

        private Animator animator;

        private HitStopCachedState hitStopCachedState;

        private GunlanceShellController gunlanceShellController;
        private GunlanceShellController.ShellingInfo shellInfo;

        [SerializeField]
        public float minBlastSpeed = 20f;

        [SerializeField]
        public float maxBlastSpeed = 35f;

        [SerializeField]
        public float minBlastForce = 500f;

        [SerializeField]
        public float maxBlastForce = 5000f;


        public static bool disableAirControlUntilCollision;

        public static float speedCoefficientOnExit;

        public static float velocityDamageCoefficient = .3f;

        protected Vector3 blastVelocity;

        private float bonusDamage;

        public float blastSpeed { get; private set; }

        private string beginSwingSoundString;
        private GameObject swingEffectPrefab = GlaiveAssets.swordSwingEffect;
        private GameObject swingEffectInstance;
        protected EffectManagerHelper _emh_swingEffectInstance;
        protected string swingEffectMuzzleString = "BlastDashHitbox";
        private Run.FixedTimeStamp meleeAttackStartTime;

        public override void OnEnter()
        {
            base.OnEnter();
            base.skillLocator.secondary.DeductStock(1);
            gunlanceShellController = base.GetComponent<GunlanceShellController>();
            if (gunlanceShellController)
            {
                shellInfo = gunlanceShellController.ReturnShellingInfo();
            }

            switch (shellInfo.type)
            {
                case GunlanceShellController.ShellType.normalShell:
                    duration = .5f;
                    break;
                case GunlanceShellController.ShellType.longShell:
                    duration = .75f;
                    break;
                case GunlanceShellController.ShellType.wideShell:
                    duration = .9f;
                    break;
            }
            uninterruptibleDuration = duration * .25f;

            if (base.isAuthority)
            {
                base.characterMotor.Motor.ForceUnground();
                base.characterMotor.disableAirControlUntilCollision |= disableAirControlUntilCollision;
                blastVelocity = CalculateLungeVelocity(base.characterMotor.velocity, GetAimRay().direction, duration, minBlastSpeed, maxBlastSpeed);
                base.characterMotor.velocity = blastVelocity;
                base.characterDirection.forward = base.characterMotor.velocity.normalized;
                blastSpeed = base.characterMotor.velocity.magnitude;
                bonusDamage = blastSpeed * (velocityDamageCoefficient * damageStat);
            }


            animator = GetModelAnimator();
            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = GetModelTransform();
            string hitboxString = "BlastDashGroup";
            if ((bool)modelTransform)
            {
                hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == hitboxString);
            }
            float num = damageCoefficient;
            attack = new OverlapAttack();
            attack.damageType = DamageTypeCombo.GenericSecondary;
            attack.attacker = base.gameObject;
            attack.inflictor = base.gameObject;
            attack.teamIndex = GetTeam();
            attack.damage = num * base.characterBody.damage + bonusDamage;
            attack.procCoefficient = 1f;
            attack.hitEffectPrefab = null;
            attack.forceVector = base.characterMotor.velocity + GetAimRay().direction * Mathf.Lerp(minBlastForce, maxBlastForce, duration);
            attack.pushAwayForce = 500f;
            attack.hitBoxGroup = hitBoxGroup;
            attack.isCrit = RollCrit();
            //attack.impactSound = "";
        }

        public override void OnExit()
        {
            base.OnExit();
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
        }

        public void FireAttack()
        {
            if (!hasFired)
            {
                hasFired = true;
            }
            if (base.isAuthority)
            {
                BeginMeleeAttackEffect();
                Ray aimRay = GetAimRay();
                if (attack.Fire() && !inHitPause)
                {
                    //hitStopCachedState = CreateHitStopCachedState(base.characterMotor, animator, "Whirlwind.playbackRate");
                    hitPauseTimer = 4f * GroundLight.hitPauseDuration / attackSpeedStat;
                    inHitPause = true;
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();        
            stopwatch += Time.fixedDeltaTime;
            FireAttack();
            if (!inHitPause)
            {
                Vector3 vector = Vector3.RotateTowards(blastVelocity.normalized, base.GetAimRay().direction.normalized, 1f, 5f).normalized;
                base.characterMotor.velocity = vector * blastVelocity.magnitude;
                base.characterDirection.forward = vector;
                base.characterBody.isSprinting = true;
            }
            if (base.isAuthority && base.fixedAge > duration)
            {
                outer.SetNextStateToMain();
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

        public static Vector3 CalculateLungeVelocity(Vector3 currentVelocity, Vector3 aimDirection, float charge, float minLungeSpeed, float maxLungeSpeed)
        {
            currentVelocity = ((Vector3.Dot(currentVelocity, aimDirection) < 0f) ? Vector3.zero : Vector3.Project(currentVelocity, aimDirection));
            return currentVelocity + aimDirection * Mathf.Lerp(minLungeSpeed, maxLungeSpeed, charge);
        }


        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (base.fixedAge < uninterruptibleDuration)
            {
                return InterruptPriority.PrioritySkill;
            }
            return InterruptPriority.Skill;
        }

    }
}
