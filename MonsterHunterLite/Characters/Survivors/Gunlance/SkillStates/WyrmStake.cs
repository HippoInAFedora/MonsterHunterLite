using EntityStates;
using UnityEngine;
using System;
using RoR2;
using MonsterHunterMod.Characters.Survivors.Gunlance.Components;
using MonsterHunterMod.Survivors.Gunlance;
using UnityEngine.Networking;
using RoR2.Projectile;
using R2API;
using RoR2.Skills;
using EntityStates.Toolbot;

namespace MonsterHunterMod.Characters.Survivors.Gunlance.SkillStates
{
    internal class WyrmStake : BaseSkillState, SteppedSkillDef.IStepSetter
    {
        public int step;

        public ChildLocator childLocator;

        public GunlanceShellController shell;

        public Transform shellTransform;

        public Vector3 idealPos;

        //public GameObject wyrmStakePrefab = GunlanceAssets.wyrmStake;
        public static GameObject tracerEffectPrefab = GunlanceAssets.wyrmStake;
        public static GameObject hitEffectPrefab = GunlanceAssets.wyrmStakeImpact;

        public float baseDuration = .5f;
        public float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            base.skillLocator.primary.DeductStock(1);    
            duration = baseDuration / base.attackSpeedStat;    
            shell = base.GetComponent<GunlanceShellController>();
            if (base.isAuthority)
            {
                BulletAttack bullet = new BulletAttack();
                bullet.owner = base.gameObject;
                bullet.weapon = base.gameObject;
                bullet.origin = base.GetAimRay().origin;
                bullet.aimVector = base.GetAimRay().direction;
                bullet.force = 500f;
                bullet.radius = .5f;
                bullet.bulletCount = 1;
                bullet.stopperMask = LayerIndex.CommonMasks.bullet;
                bullet.isCrit = base.RollCrit();
                bullet.procCoefficient = 1f;
                bullet.maxSpread = 0f;
                bullet.minSpread = 0f;
                bullet.maxDistance = 500f;
                bullet.damage = 0f;
                bullet.hitEffectPrefab = hitEffectPrefab;
                bullet.tracerEffectPrefab = tracerEffectPrefab;
                bullet.AddModdedDamageType(GunlanceDamage.wyrmStakeDamage);
                bullet.Fire();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            
            if (base.isAuthority && base.fixedAge > duration)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }

        public void SetStep(int i)
        {
            step = i;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write((byte)step);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            step = reader.ReadByte();
        }
    }
}
