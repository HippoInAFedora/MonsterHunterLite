using EntityStates;
using RoR2;
using MonsterHunterMod.Characters.Survivors.Gunlance.Components;
using MonsterHunterMod.Survivors.Glaive;
using MonsterHunterMod.Survivors.Gunlance;
using UnityEngine;
using UnityEngine.Networking;
using RoR2.Skills;
using UnityEngine.AddressableAssets;

namespace MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates
{
    internal class WyvernFire : BaseSkillState
    {
        //baseDuration here controls percentages, not a full duration aspect.
        public float baseDuration = 6f;
        public float thrustDelayPercentTime = .67f;
        public float duration;

        public static GameObject tracerEffectPrefab = GunlanceAssets.wyvernFire;

        public float stopwatch = 0f;
        private bool flag = false;

        private float cachedMoveSpeed;

        public override void OnEnter()
        {
            duration = baseDuration / attackSpeedStat;
            base.OnEnter();
            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(GunlanceBuffs.gunlanceGuardBuff);
            }
        }
        public override void OnExit()
        {
            base.OnExit();
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(GunlanceBuffs.gunlanceGuardBuff);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.characterMotor)
            {
                if (base.fixedAge < duration * thrustDelayPercentTime)
                {
                    base.characterMotor.velocity.x = 0f;
                    base.characterMotor.velocity.z = 0f;
                }
            }
            if (base.isAuthority)
            {
                if (base.fixedAge > duration * thrustDelayPercentTime && !flag)
                {
                    Fire();
                    if (characterMotor)
                    {
                        base.characterMotor.velocity.x = GetAimRay().direction.x * Mathf.Lerp(-50, -5, stopwatch);
                        base.characterMotor.velocity.z = GetAimRay().direction.z * Mathf.Lerp(-50, -5, stopwatch);
                    }
                    flag = true;
                }
                if (base.fixedAge > duration)
                {
                    outer.SetNextStateToMain();
                }
            }

        }

        public void Fire()
        {
            base.SmallHop(characterMotor, 4f);
            base.AddRecoil(5, 5, 5, 5);
            BulletAttack bullet = new BulletAttack();
            bullet.owner = base.gameObject;
            bullet.weapon = base.gameObject;
            bullet.origin = base.GetAimRay().origin;
            bullet.aimVector = base.GetAimRay().direction;
            bullet.force = 500f;
            bullet.radius = 12f;
            bullet.bulletCount = 1;
            bullet.stopperMask = LayerIndex.noCollision.mask;
            bullet.isCrit = base.RollCrit();
            bullet.procCoefficient = 1f;
            bullet.maxSpread = 0f;
            bullet.minSpread = 0f;
            bullet.maxDistance = 200f;
            bullet.damage = base.characterBody.damage * GunlanceStaticValues.wyvernFireCoefficient;
            bullet.tracerEffectPrefab = tracerEffectPrefab;
            bullet.Fire();
        }
        
    }
}

