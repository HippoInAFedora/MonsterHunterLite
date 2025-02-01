using EntityStates;
using RoR2;
using MonsterHunterMod.Characters.Survivors.Greatsword.Components;
using UnityEngine.Networking;
using UnityEngine;
using MonsterHunterMod.Survivors.Greatsword;

namespace MonsterHunterMod.Characters.Survivors.Greatsword.SkillStates
{
    public class Slam : BaseState
    {
        public int charge;

        public static float airControl;

        public static float minimumDuration;

        public static float blastRadius;

        public static float blastProcCoefficient;

        public static float blastDamageCoefficient;

        public static float blastForce;

        public static string enterSoundString;

        public static float initialVerticalVelocity;

        public static float exitVerticalVelocity;

        public static float verticalAcceleration;

        public static float exitSlowdownCoefficient;

        public static Vector3 blastBonusForce;

        public static GameObject blastImpactEffectPrefab;

        public static GameObject blastEffectPrefab;

        public static float normalizedVFXPositionBetweenFootAndClub;

        public static float normalizedBlastPositionBetweenFootAndClub;

        public static float smallHopVelocity;

        public static float heightInfluencePercent;

        private float previousAirControl;

        private float timeInAir;

        private float airClubTimer;

        private bool hitTheGround;

        private float delayBeforeExit;

        private bool detonateNextFrame;
        //baseDuration here controls percentages, not a full duration aspect.
        public float baseDuration = 1.2f;
        public float slamDelayPercentTime = .25f;
        public float slamCompletePercentTime = .4f;
        public float duration;

        public float stopwatch = 0f;
        public GreatswordChargeController controller;
        public bool hasSlammed;

        public override void OnEnter()
        {
            duration = baseDuration / attackSpeedStat;
            controller = base.GetComponent<GreatswordChargeController>();
            if (controller != null )
            {
                charge = controller.charge == GreatswordChargeController.ChargeLevel.Overcharged ? 2 : (int)controller.charge;
            }
            base.OnEnter();
        }
        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            float deltaTime = GetDeltaTime();
            if (base.isAuthority && (bool)base.characterMotor)
            {
                base.characterMotor.moveDirection = base.inputBank.moveVector;
                base.characterDirection.moveVector = base.characterMotor.moveDirection;
            }
            if (NetworkServer.active)
            {
                base.characterBody.AddTimedBuff(JunkContent.Buffs.IgnoreFallDamage, 0.25f, 1);
            }
        }

        protected BlastAttack.Result DetonateAuthority()
        {
            Vector3 position = base.characterBody.corePosition;
            Vector3 footPosition = base.characterBody.footPosition;
            Vector3 origin = footPosition + (position - footPosition) * normalizedVFXPositionBetweenFootAndClub;
            Vector3 position2 = footPosition + (position - footPosition) * normalizedBlastPositionBetweenFootAndClub;
            EffectManager.SpawnEffect(blastEffectPrefab, new EffectData
            {
                origin = origin,
                scale = blastRadius * (charge + 1.25f)
            }, transmit: true);
            return new BlastAttack
            {
                attacker = base.gameObject,
                baseDamage = base.characterBody.damage * GreatswordStaticValues.slamDamageCoefficient * (1 + GreatswordStaticValues.chargeMultiplier * charge),
                baseForce = blastForce,
                bonusForce = blastBonusForce,
                crit = RollCrit(),
                falloffModel = BlastAttack.FalloffModel.None,
                procCoefficient = blastProcCoefficient,
                radius = blastRadius * (charge + 1.25f),
                position = position2,
                attackerFiltering = AttackerFiltering.NeverHitSelf,
                impactEffect = EffectCatalog.FindEffectIndexFromPrefab(blastImpactEffectPrefab),
                teamIndex = base.teamComponent.teamIndex
            }.Fire();
        }

    }
}

